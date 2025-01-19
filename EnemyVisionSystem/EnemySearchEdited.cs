using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Handles the enemy's vision, is run at all times on a delay, used as trigger for EnemyChaseState
[Serializable]
public class EnemySearch
{
    public enum TargetVisionState { Unseen, Seen, Caught }

    private EnemyStateMachiene _ctx;
    private Transform _targetTransform;
    private LayerMask _mask;
    private string _targetTag;
    [SerializeField] private float _radius = 10;
    private float _chaseRadius = 1.6f;
    [SerializeField] private float _angle = 60; // Controls the angle of the enemy's vision
    
    private TargetVisionState _targetVState = TargetVisionState.Unseen;
    private CharacterController _playerCtrl;
    private Vector3 _heightOffset = Vector3.zero;
    private float _radiusOffset;
    private Vector3 _posLastSeen = Vector3.zero;
    private float _timeLastSeen = 0f;
    private PlayerNoiseLevel _playerNoiseLevel;
    private CharacterSwapController _characterSwapController;
    [SerializeField] private float _noiseDistanceQuiet = 0f, _noiseDistanceNormal = 5f, _noiseDistanceLoud = 15f;
    [SerializeField] private GameObject alertIcon;
    private AlertIcon _thisAlertIcon;
    [SerializeField] private float _alertIconHeightOffset;
    private List<IEnemyListener> _listeners = new List<IEnemyListener>();
    private bool spotted = false;
    private AlertSpeed _alertSpeed;
    private bool _enteredForbiddenArea = false;

    [Header("Detection Percentages")]
    [SerializeField] private float _percentFar = 0.8f;
    [SerializeField] private float _percentMid = 0.4f;
    [SerializeField] private float _percentClose = 0.12f;

    [Header("Detection Speeds (% per sec)")]
    [SerializeField] private float _regressingSpeed = -0.2f;
    [SerializeField] private float _speedFar = 0.75f;
    [SerializeField] private float _speedMidFar = 1.5f;
    [SerializeField] private float _speedCloseMid = 2;
    [SerializeField] private float _speedClose = 3;

    public bool EneteredForbiddenArea { set { _enteredForbiddenArea = value; } }
    //...

    public TargetVisionState TargetVState
    {
        get { return _targetVState; }
        set { foreach (IEnemyListener l in _listeners) { l.OnUpdateTargetVisionState(value); } _targetVState = value; }
    }

    public Vector3 PosLastSeen { get { return _posLastSeen; } }

    public void Initialize(EnemyStateMachiene ctx, string targetTag)
    {
        //...

        _mask = LayerMask.GetMask("Player", "EnemySight");
        _targetTag = targetTag;
        _ctx = ctx;
        _targetTransform = GameObject.FindGameObjectWithTag(_targetTag).transform;
        _playerCtrl = _targetTransform.GetComponent<CharacterController>();
        _playerNoiseLevel = _targetTransform.GetComponent<PlayerStateMachine>().PlayerNoiseLevel;
        _characterSwapController = _targetTransform.parent.Find("pre_PlayerCharacter").GetComponent<CharacterSwapController>();
        _heightOffset = new Vector3(0, _ctx.GetComponent<NavMeshAgent>().height, 0);
        _radiusOffset = _ctx.GetComponent<NavMeshAgent>().radius;
        _chaseRadius += _radiusOffset * 2f;
        _ctx.StartCoroutine(FindPlayer());
    }

    // Checks the enemy's vision to see if the player is within it on a delay
    private IEnumerator FindPlayer()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        _alertSpeed = AlertSpeed.NegativeSlow;
        spotted = false;
        Coroutine alertRoutine = null;

        while (_targetVState == TargetVisionState.Unseen)
        {
            Vector3 dir = ((_targetTransform.position + new Vector3(0, (((_playerCtrl.height * 0.75f) / 2) + _playerCtrl.center.y) * _targetTransform.localScale.y, 0)) - (_ctx.transform.position + _heightOffset)).normalized;
            Vector3 angleDir = new Vector3(dir.x, 0, dir.z);

            Debug.DrawRay(_ctx.transform.position + _heightOffset, _radius * dir, Color.cyan, 0.2f); // Raycast Ray

            // Raycasts a ray towards the player and checks if it's hit a player or obstacle and if the player is within the angle of the enemy's vision
            // Also detects player by distance depending on the level of noise
            if ((Physics.Raycast(_ctx.transform.position + _heightOffset, dir, out RaycastHit hitInfo, _radius, _mask) && hitInfo.transform.CompareTag(_targetTag) && Vector3.Angle(_ctx.transform.forward, angleDir) < _angle / 2)
                || Vector3.Distance(_ctx.transform.position, _targetTransform.position) < GetNoiseDistance()
                || _enteredForbiddenArea)
            {
                if (_thisAlertIcon == null)
                {
                    _thisAlertIcon = UnityEngine.Object.Instantiate(alertIcon, _ctx.gameObject.transform.position + Vector3.up * _alertIconHeightOffset, Quaternion.identity, _ctx.gameObject.transform).GetComponent<AlertIcon>();
                    _thisAlertIcon.DetectionSpeeds = this.DetectionSpeeds;
                    alertRoutine = _ctx.StartCoroutine(TickUpAlertness(_thisAlertIcon));
                    if (_ctx.EnemyAnimator.TryGetComponent<AudioByName>(out AudioByName enemyAudio))
                        enemyAudio.Play("Alerted");
                }

                float dist = Vector3.Distance(_ctx.transform.position, _targetTransform.position) / _radius;

                switch (dist) // AlertSpeed based on player distance from the enemy, with radius as basis
                {
                    case float n when (n > _percentFar):
                        _alertSpeed = AlertSpeed.Slow;
                        break;
                    case float n when (n > _percentMid):
                        _alertSpeed = AlertSpeed.Medium;
                        break;
                    case float n when (n > _percentClose):
                        _alertSpeed = AlertSpeed.Fast;
                        break;
                    default:
                        _alertSpeed = AlertSpeed.Extreme;
                        break;
                }

                if (_enteredForbiddenArea)
                   _alertSpeed = AlertSpeed.Extreme;
            }
            else
                _alertSpeed = AlertSpeed.NegativeSlow;

            if (spotted)
            {
                TargetVState = TargetVisionState.Seen;
                _ctx.StopCoroutine(alertRoutine);
                alertRoutine = null;
                _ctx.StartCoroutine(ChasePlayer());
                break;
            }

            yield return wait;
        }
    }

    // Function increasing/decreasing enemy detection progress
    private IEnumerator TickUpAlertness(AlertIcon alertIcon)
    {
        yield return new WaitForSeconds(0.05f); // Wait for object material to initialize

        while (_thisAlertIcon != null)
        {
            spotted = alertIcon.FillIcon(_alertSpeed);
            yield return new WaitForSeconds(0.05f);
        }
    }

    // Checks player position and tracks if the player has been caught or not
    private IEnumerator ChasePlayer()
    {
        _characterSwapController.EnemyChaserNum++;
        WaitForSeconds wait = new WaitForSeconds(0.1f);

        _posLastSeen = _targetTransform.position; // makes sure enemy runs to where they first spotted player, even if they are out of sight for raycast

        while (_targetVState == TargetVisionState.Seen)
        {            
            Vector3 dir = (_targetTransform.position - (_ctx.transform.position + (_heightOffset / 2))).normalized;
            Vector3 startPoint =  (_radiusOffset * -new Vector3(dir.x, 0, dir.y)) + (_ctx.transform.position + (_heightOffset / 2));
            dir = (_targetTransform.position - startPoint).normalized;
            
            Debug.DrawRay(startPoint, dir * _chaseRadius, Color.magenta, 3f);

            // Raycasts a ray towards the player and checks if it's hit a player or obstacle
            if (Physics.Raycast(startPoint, dir, out RaycastHit hitInfo, _chaseRadius, _mask) && hitInfo.transform.CompareTag(_targetTag))
            { // Raycast may fail due to range being too short for difference in height, I.E. raycast goes downwards or upwards causing range to be incorrect
                TargetVState = TargetVisionState.Caught;

                if (!GameManager.Instance.LoadFailed)
                    GameManager.Instance.RestartFromPreviousCheckpoint();
                else
                    ScreenEffects.TriggerFadeOut(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name); // Fades out screen and restarts level

                break;
            }

            // Checks the players' last known location and time
            if ((Physics.Raycast(_ctx.transform.position, dir, out RaycastHit rayHit, _radius, _mask) && rayHit.transform.CompareTag(_targetTag))
                || Vector3.Distance(_ctx.transform.position, _targetTransform.position) < GetNoiseDistance())
            {
                _posLastSeen = _targetTransform.position;
                _timeLastSeen = Time.time;
            }
            else if (Time.time - _timeLastSeen < 1f) // Short tracking of player position after enemy has lost sight
            {
                _posLastSeen = _targetTransform.position;
            }

            yield return wait;
        }

        _characterSwapController.EnemyChaserNum--;
        GameObject.Destroy(_thisAlertIcon.gameObject);
        _ctx.StartCoroutine(FindPlayer());
    }

    private float GetNoiseDistance()
    {
        return _playerNoiseLevel.Noiselvl switch
        {
            PlayerNoiseLevel.NoiseLevel.Quiet => _noiseDistanceQuiet,
            PlayerNoiseLevel.NoiseLevel.Normal => _noiseDistanceNormal,
            PlayerNoiseLevel.NoiseLevel.Loud => _noiseDistanceLoud,
            _ => 0,
        };
    }

    public void AddListener(IEnemyListener listener)
    {
        _listeners.Add(listener);
    }
}
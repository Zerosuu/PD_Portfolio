using UnityEngine;
using System.Collections;

public class PlayerClimbState : PlayerBaseState, IImportComponentToState
{
    private Transform _ladderTransform;
    private float _currentRotationVelocity;
    private float _rotationTime = 0.1f;
    private float _climbSpeedMultiplier = 0.7f;
    private bool _edgeTrigger = false;
    private bool _startFrame = true;

    public PlayerClimbState(PlayerStateMachiene currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (Input.GetKeyDown(KeyCode.E) && !_startFrame)
            SwitchState(_factory.Idle());
    }

    public override void EnterState()
    {
        _ctx.PlayerAnimator.CrossFadeInFixedTime("Base Layer.Climbing", 0.2f);
        Import();
        _ctx.StartCoroutine(RotateToLadder());
        _startFrame = true;
    }

    public override void ExitState()
    {
        if (!_edgeTrigger)
            _ctx.PlayerAnimator.Play("Base Layer.anim_gooberIdle");

        _edgeTrigger = false;
    }

    public override void TakeMessage(string message) 
    {
        if (message == "TriggerEdgeClimb")
        {
            _edgeTrigger = true;
            SwitchState(_factory.EdgeClimb());
        }
    }

    public override void UpdateState()
    {
        CheckSwitchStates();

        HandleClimb();

        if (_startFrame) _startFrame = false;
    }

    // Handles climbing logic
    private void HandleClimb()
    {
        _ctx.CharacterController.Move(_ctx.MovementSpeed * _climbSpeedMultiplier * Time.deltaTime * Vector3.up);
    }

    // Imports information to class, ladder's transform
    public void Import()
    {
        try
        {
            _ladderTransform = (Transform)_ctx.ImportInfo;
            _ctx.ImportInfo = null;
        }
        catch
        {
            Debug.Log("PlayerClimbState: Component is not a Transform");
            SwitchState(_factory.Idle());
        }
    }

    private IEnumerator RotateToLadder()
    {
        float targetYRotation = _ladderTransform.localRotation.eulerAngles.y;
        float angle = 0f;

        while (!(_ctx.PlayerModelTransform.eulerAngles.y < targetYRotation + 5 && _ctx.PlayerModelTransform.eulerAngles.y > targetYRotation - 5))
        {
            angle = Mathf.SmoothDampAngle(_ctx.PlayerModelTransform.eulerAngles.y, targetYRotation, ref _currentRotationVelocity, _rotationTime);
            _ctx.PlayerModelTransform.rotation = Quaternion.Euler(0f, angle, 0f);
            yield return null;
        }

        _ctx.PlayerModelTransform.rotation = Quaternion.Euler(0f, targetYRotation, 0f);
    }
}

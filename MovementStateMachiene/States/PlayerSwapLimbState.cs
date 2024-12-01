using System.Collections;
using UnityEngine;

// Class handling the limb swapping for the player
public class PlayerSwapLimbState : PlayerBaseState, IImportComponentToState
{
    private InteractSpriteDialogue _dialogue; // Keeps a reference to the script handling information about dialogue
    private bool _playerMoved = false;
    private float _rotationTime = 0.1f;
    private float _currentRotationVelocity;

    private Transform _newLimbTransform;

    public PlayerSwapLimbState(PlayerStateMachiene currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (_ctx.AnimFinished)
            SwitchState(_factory.Dialogue());
    }

    public override void EnterState()
    {
        Import();

        _newLimbTransform = _ctx.transform.Find("ModelParent/me_goober/Goober/" + _dialogue.transform.GetChild(1).name);
        if (_dialogue.transform.GetChild(1).TryGetComponent<ParentLimb>(out ParentLimb parentLimb))
            parentLimb.SetParent(_ctx.transform);       

        _ctx.StartCoroutine(MovePlayer());
    }

    public override void ExitState()
    {
        // Avoids setting the new limb to current if it is an inferior variant,
        // used for exception in section 6 when picking up tentacle arm after throwing arm
        if (!_ctx.GetComponent<LimbManager>().SuperiorCurrentLimb(_newLimbTransform.GetComponent<Limbs>()))
            _ctx.GetComponent<LimbManager>().CurrentLimb = _newLimbTransform.GetComponent<Limbs>();
        else
            _dialogue.transform.GetChild(1).gameObject.SetActive(false);
        
        _ctx.AnimFinished = false;

        // Deactivates camera event for the limb swap if it exists
        if (_dialogue.transform.TryGetComponent<ChangeCameraTemporarily>(out ChangeCameraTemporarily camChange))
        {
            camChange.Activate(false);
        }

        _dialogue.TriggerDialogue();

        // Triggers tutorial prompt for how to use new arm
        if (_dialogue.transform.GetChild(1).name == "GooberGetElectricArm")
            _ctx.GetComponent<ButtonPromptScript>().ActivateArmPrompt("Electric");
        else if (_dialogue.transform.GetChild(1).name == "GooberGetThrowArm")
            _ctx.GetComponent<ButtonPromptScript>().ActivateArmPrompt("Throw");
    }

    public override void TakeMessage(string message) { }

    public override void UpdateState()
    {
        CheckSwitchStates();

        if (_playerMoved)
        {
            _playerMoved = false;
            _dialogue.transform.GetChild(1).GetComponent<Animator>().Play("Base Layer.ArmSwap");

            // Special else case for second time getting tentacle arm
            if (!_ctx.GetComponent<LimbManager>().SuperiorCurrentLimb(_newLimbTransform.GetComponent<Limbs>()))
                _ctx.PlayerAnimator.Play("Base Layer." + _dialogue.transform.GetChild(1).name);
            else
                _ctx.PlayerAnimator.Play("Base Layer." + _dialogue.transform.GetChild(1).name + "1");

            // Activates camera event for the limb swap if it exists
            if (_dialogue.transform.TryGetComponent<ChangeCameraTemporarily>(out ChangeCameraTemporarily camChange))
            {
                camChange.Activate(true);
            }
        }

        // Activates arm om player model at right moment
        if (_ctx.OnLimbSwap)
        {
            _ctx.GetComponent<LimbManager>().SwapLimb(_newLimbTransform);
            _ctx.OnLimbSwap = false;
        }
    }

    public void Import()
    {
        try
        {
            _dialogue = (InteractSpriteDialogue)_ctx.ImportInfo;
        }
        catch
        {
            Debug.Log("Component is not a DialogueInteraction");
        }
    }

    // Coroutine handling correct position and rotation for the player when starting limb swap
    private IEnumerator MovePlayer()
    {
        Transform targetTransform = _dialogue.transform.Find("PlayerPos");
        Vector3 playerStartPos = _ctx.transform.position;
        Vector3 targetPosition = new Vector3(targetTransform.position.x, targetTransform.position.y + _ctx.CharacterController.height / 2, targetTransform.position.z);
        float distanceToTarget = Vector3.Distance(playerStartPos, targetPosition);

        float targetYRotation = targetTransform.localRotation.eulerAngles.y;
        float angle = 0f;

        Vector3 dir = (targetPosition - _ctx.transform.position).normalized;
        float speed = Vector3.Distance(_ctx.transform.position, targetPosition) / 0.2f;

        // Stops the loop when player has moved to the correct point or further
        while (Vector3.Distance(_ctx.transform.position, playerStartPos) < distanceToTarget)
        {
            angle = Mathf.SmoothDampAngle(_ctx.PlayerModelTransform.localRotation.eulerAngles.y, targetYRotation, ref _currentRotationVelocity, _rotationTime);
            _ctx.PlayerModelTransform.localRotation = Quaternion.Euler(0f, angle, 0f);

            _ctx.transform.position += speed * Time.deltaTime * dir;
            yield return null;
        }

        // Snaps the player to correct angle and position, in case of slight misalignment
        _ctx.PlayerModelTransform.localRotation = Quaternion.Euler(0f, targetYRotation, 0f);
        _ctx.transform.position = targetPosition;

        _playerMoved = true; // Allows limb swap to continue when player is placed in the correct position and angle
    }
}
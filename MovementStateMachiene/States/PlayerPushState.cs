using UnityEngine;

public class PlayerPushState : PlayerBaseState, IImportComponentToState
{
    private InteractPushObject _pushObjectScript;    
    float pushSpeedMultiplier = 0.7f;
    private float _currentRotationVelocity;
    private float _rotationTime = 0.1f;

    public PlayerPushState(PlayerStateMachiene currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }
    
    public override void CheckSwitchStates()
    {
        if (!_pushObjectScript.Active)
        {
            _ctx.InPushCollider = false;
            SwitchState(_factory.Idle());
        }
        else if(!Input.GetKey(_pushObjectScript.DirectionalKey) || _pushObjectScript.Blocked)
        {
            SwitchState(_factory.Idle());
        }        
    }

    public override void EnterState()
    {
        Import();

        _ctx.PlayerAnimator.CrossFadeInFixedTime("Base Layer.gooberPush", 0.2f);

        if (_pushObjectScript.transform.parent.transform.parent.TryGetComponent<AudioMoving>(out AudioMoving pushAudio))
            pushAudio.enabled = true;
    }

    public override void ExitState()
    {
        _ctx.PlayerAnimator.CrossFadeInFixedTime("Base Layer.anim_gooberIdle", 0.2f);
    }

    public override void TakeMessage(string message) {}

    public override void UpdateState()
    {
        CheckSwitchStates();

        if (!_pushObjectScript.Blocked)
        {
            // Moves the player and the object with the same velocity and direction
            _ctx.CharacterController.SimpleMove(_ctx.MovementSpeed * pushSpeedMultiplier * _pushObjectScript.PushDirection);
            _pushObjectScript.RotationParent.parent.position += (_ctx.MovementSpeed * pushSpeedMultiplier * Time.deltaTime * _pushObjectScript.PushDirection);

            // Rotates the player towards the object being pushed
            float targetAngle = Mathf.Atan2(_pushObjectScript.PushDirection.x, _pushObjectScript.PushDirection.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(_ctx.PlayerModelTransform.eulerAngles.y, targetAngle, ref _currentRotationVelocity, _rotationTime);
            _ctx.PlayerModelTransform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
    }

    public void Import()
    {
        try
        {
            _pushObjectScript = (InteractPushObject)_ctx.ImportInfo;
        }
        catch
        {
            Debug.Log("PlayerPushState: Component not a InteractPushObject");
            SwitchState(_factory.Idle());
        }
    }
}

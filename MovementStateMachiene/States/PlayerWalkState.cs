using UnityEngine;

// Class handling walking and sprinting of the player
public class PlayerWalkState : PlayerBaseState, IImportComponentToState
{
    private float _currentRotationVelocity;
    private float _rotationTime = 0.1f;
    private float _sprintMultiplier = 1.44f;
    private InteractPushObject _pushObjectScript;
    private bool _sprinting = false;

    public PlayerWalkState(PlayerStateMachiene currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (!_ctx.IsGrounded) 
        {
            // Saves the last direction of movement, used in PlayerFallingState
            _ctx.LastMovementDir = _ctx.PlayerModelTransform.forward;
            SwitchState(_factory.Falling());
        }
        else if (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0)
            SwitchState(_factory.Idle());
        else if (_ctx.InPushCollider)
        {
            Import();
            if (Input.GetKey(_pushObjectScript.DirectionalKey) && _pushObjectScript.PushPosition(_ctx.transform.position) && !_pushObjectScript.Blocked)
                SwitchState(_factory.Push());
        }
    }

    public override void EnterState()
    {
        _ctx.PlayerAnimator.SetBool("Walk", true);

        // Sets the player to sprinting if left shift is held while entering state
        if (Input.GetKey(KeyCode.LeftShift))
        { 
            _sprinting = Input.GetKey(KeyCode.LeftShift);
            _ctx.PlayerAnimator.CrossFadeInFixedTime("Base Layer.gooberRun", 0.2f);
        }
    } 

    public override void ExitState()
    {
        _ctx.PlayerAnimator.SetBool("Walk", false);
        _ctx.PlayerAnimator.CrossFadeInFixedTime("Base Layer.anim_gooberIdle", 0.2f);
    }

    public override void TakeMessage(string message) 
    {
        switch (message)
        {
            case "Dialogue":
                SwitchState(_factory.Dialogue());
                break;
            case "Climb":
                SwitchState(_factory.Climb());
                break;
            case "EdgeClimb":
                SwitchState(_factory.EdgeClimb());
                break;
        }
    }

    public override void UpdateState()
    {
        CheckSwitchStates();

        HandleMovement();
    }

    // Handles the logic used for moving the character
    private void HandleMovement()
    {
        // Sets relevant bool and animation for if the player should sprint or walk
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _sprinting = true;
            _ctx.PlayerAnimator.CrossFadeInFixedTime("Base Layer.gooberRun", 0.2f);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _sprinting = false;
            _ctx.PlayerAnimator.CrossFadeInFixedTime("Base Layer.gooberWalk", 0.2f);
        }

        // Calculates the direction the player should move in
        Vector3 dir = Vector3.zero;
        dir += _ctx.transform.forward * Input.GetAxis("Vertical");
        dir += _ctx.transform.right * Input.GetAxis("Horizontal");
        
        dir = dir.normalized;

        // Rotates the player to the correct angle
        float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(_ctx.PlayerModelTransform.eulerAngles.y, targetAngle, ref _currentRotationVelocity, _rotationTime);
        _ctx.PlayerModelTransform.rotation = Quaternion.Euler(0f, angle, 0f);

        _ctx.CharacterController.SimpleMove((_sprinting ? _sprintMultiplier : 1) * _ctx.MovementSpeed * dir);
    }

    public void Import()
    {
        try
        {
            _pushObjectScript = (InteractPushObject)_ctx.ImportInfo;
        }
        catch
        {
            Debug.Log("PlayerWalkState: ImportInfo is not a InteractPushObject");
            SwitchState(_factory.Climb());
        }
    }
}

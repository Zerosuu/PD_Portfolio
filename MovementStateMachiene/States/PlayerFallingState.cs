using UnityEngine;

public class PlayerFallingState : PlayerBaseState
{
    private Vector3 _fallVelocity = Vector3.zero;
    private Vector3 _horizontalVelocity; // Tracks how much the player should move horizontally based on which direction the player last moved
    private readonly float _horizontalMultiplier = 0.7f;
    private readonly float _horizontalFalloff = 0.9f; // Multiplier for how much horizontal velocity should be retained each frame

    public PlayerFallingState(PlayerStateMachiene currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (_ctx.IsGrounded)
            SwitchState(_factory.Idle());
    }

    public override void EnterState()
    {
        _ctx.PlayerAnimator.CrossFadeInFixedTime("gooberFalling", 0.2f);

        // Gets the last direction the player moved
        _horizontalVelocity = _ctx.LastMovementDir * (_horizontalMultiplier * 0.5f);
        _ctx.LastMovementDir = Vector3.zero;
    }

    public override void ExitState()
    {
        _ctx.PlayerAnimator.CrossFadeInFixedTime("anim_gooberIdle", 0.2f);
        _fallVelocity = Vector3.zero;
        _ctx.PlayerAnimator.transform.GetComponent<AudioRandom>().PlayRandomSoundEffect();
    }

    public override void TakeMessage(string message) { }

    public override void UpdateState()
    {
        CheckSwitchStates();

        HandleGravity();
    }

    // Handles the logic used for falling
    private void HandleGravity()
    {
        if (Time.timeScale != 0)
            _fallVelocity += (Physics.gravity * _ctx.GravityMultiplier) + (_horizontalVelocity * _horizontalMultiplier);
        _horizontalVelocity *= _horizontalFalloff;
        _ctx.CharacterController.Move(_fallVelocity * Time.deltaTime);
    }
}

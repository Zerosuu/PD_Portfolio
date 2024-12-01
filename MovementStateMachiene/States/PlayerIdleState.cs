using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachiene currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (!_ctx.IsGrounded)
            SwitchState(_factory.Falling());
        else if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            SwitchState(_factory.Walk());
    }

    public override void EnterState()
    {
        _ctx.PlayerAnimator.SetBool("Idle", true);
    }

    public override void ExitState()
    {
        _ctx.PlayerAnimator.SetBool("Idle", false);
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
    }        
}

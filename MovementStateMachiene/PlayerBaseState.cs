using UnityEngine;

// Abstract state used as base class for all player states
public abstract class PlayerBaseState
{
    protected PlayerStateMachiene _ctx;
    protected PlayerStateFactory _factory;

    public PlayerBaseState(PlayerStateMachiene currentContext, PlayerStateFactory playerStateFactory)
    {
        _ctx = currentContext;
        _factory = playerStateFactory;
    }

    public abstract void EnterState(); // Handles initial code, runs when entering the state

    public abstract void UpdateState(); // Handles code run every frame, run through PlayerStateMachiene

    public abstract void ExitState(); // Handles code run when exiting the state

    public abstract void CheckSwitchStates(); // Handles checks to see if state should be exited
    
    public abstract void TakeMessage(string message); // Function to send information to the current state

    protected void SwitchState(PlayerBaseState newState) // Handles proper switching of states
    {
        ExitState(); // Exit the current state

        newState.EnterState(); // Enter the new state

        _ctx.CurrentState = newState; // Switch the state
    }
}

// Interface used to import components from interactable objects
interface IImportComponentToState
{
    public void Import();
}

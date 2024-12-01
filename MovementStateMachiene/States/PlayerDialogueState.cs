using UnityEngine;

public class PlayerDialogueState : PlayerBaseState, IImportComponentToState
{

    private InteractSpriteDialogue _dialogue; // Keeps a reference to the script handling information about dialogue
    private bool _keepState = true;

    public PlayerDialogueState(PlayerStateMachiene currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (!_keepState || Input.GetKeyDown(KeyCode.Escape))
            SwitchState(_factory.Idle());
    }

    public override void EnterState()
    {
        Import();

        _ctx.PlayerAnimator.CrossFadeInFixedTime("Base Layer.anim_gooberIdle", 0.2f);
    }

    public override void ExitState()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            _dialogue.ExitDialogue();
        else
            _dialogue.PauseDialogue();
        _keepState = false;
    }

    public override void TakeMessage(string message) { }

    public override void UpdateState()
    {
        CheckSwitchStates();

        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
        {
            if (_dialogue.LimbSwapLine)
                SwitchState(_factory.SwapLimb());
            else
                _keepState = _dialogue.TriggerDialogue(); // Triggers one line of dialogue at a time, sets _keepstate to false when no lines are left
        }
    }

    // Imports the reference to the correct dialogue
    public void Import()
    {
        try
        {
            _dialogue = (InteractSpriteDialogue)_ctx.ImportInfo;
            _keepState = true;
        }
        catch
        {
            Debug.Log("Component is not a DialogueInteraction");
            _keepState = false;
        }
    }
}
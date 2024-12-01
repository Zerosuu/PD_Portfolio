// Creates the player's states and keeps track of the references
public class PlayerStateFactory
{
    private readonly PlayerStateMachiene _context;
    private readonly PlayerIdleState _playerIdleState;
    private readonly PlayerWalkState _playerWalkState;
    private readonly PlayerClimbState _playerClimbState;
    private readonly PlayerFallingState _playerFallingState;
    private readonly PlayerDialogueState _playerDialogueState;
    private readonly PlayerEdgeClimbState _playerEdgeClimbState;
    private readonly PlayerSwapLimbState _playerSwapLimbState;
    private readonly PlayerPushState _playerPushState;

    public PlayerStateFactory(PlayerStateMachiene currentContext)
    {
        _context = currentContext;
        _playerIdleState = new PlayerIdleState(_context, this);
        _playerWalkState = new PlayerWalkState(_context, this);
        _playerClimbState = new PlayerClimbState(_context, this);
        _playerFallingState = new PlayerFallingState(_context, this);
        _playerDialogueState = new PlayerDialogueState(_context, this);
        _playerEdgeClimbState = new PlayerEdgeClimbState(_context, this);
        _playerSwapLimbState = new PlayerSwapLimbState(_context, this);
        _playerPushState = new PlayerPushState(_context, this);
    }

    public PlayerBaseState Idle() 
    {
        return _playerIdleState;
    }

    public PlayerBaseState Walk() 
    {
        return _playerWalkState;
    }

    public PlayerBaseState Climb() 
    {
        return _playerClimbState;
    }

    public PlayerBaseState Falling() 
    {
        return _playerFallingState;
    }
    
    public PlayerBaseState Dialogue()
    {
        return _playerDialogueState;
    }

    public PlayerEdgeClimbState EdgeClimb()
    {
        return _playerEdgeClimbState;
    }

    public PlayerSwapLimbState SwapLimb()
    {
        return _playerSwapLimbState;
    }
    public PlayerPushState Push()
    {
        return _playerPushState;
    }
}

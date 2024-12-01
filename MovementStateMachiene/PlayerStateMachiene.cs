using UnityEngine;

// Handles the player's states and information between states
public class PlayerStateMachiene : MonoBehaviour
{
    private CharacterController _characterController;
    private PlayerBaseState _currentState;
    private PlayerStateFactory _states;
    private CheckGrounded _groundedScript;
    private Animator _playerAnimator; // contains controller for the character animations
    private Transform _playerModelTransform;
    private Component _importInfo = null;
    
    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _gravityMultiplier = 0.01f;
    private Vector3 _lastMovementDir; // Keeps track of the last direction moved, used in PlayerFallingState
    private bool _inPushCollider;
    private bool _animFinished = false; // Signals to the state machine when specific animations are finished, triggered through animation events
    private bool _onLimbSwap = false;

    public CharacterController CharacterController { get { return _characterController; } }
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public PlayerStateFactory States { get { return _states; } }
    public bool IsGrounded { get { return _groundedScript.IsGrounded; } }
    public Animator PlayerAnimator { get { return _playerAnimator; } }
    public Transform PlayerModelTransform { get { return _playerModelTransform; } }
    public Component ImportInfo { get { return _importInfo; } set { _importInfo = value; } }
    public float MovementSpeed { get { return _movementSpeed; } }
    public float GravityMultiplier { get { return _gravityMultiplier; } }
    public Vector3 LastMovementDir { get { return _lastMovementDir; } set { _lastMovementDir = value; } }
    public bool InPushCollider { get { return _inPushCollider; } set { _inPushCollider = value; } }
    public bool AnimFinished { get { return _animFinished; } set { _animFinished = value; } }
    public bool OnLimbSwap { get { return _onLimbSwap; } set { _onLimbSwap = value; } }

    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _groundedScript = transform.Find("GroundCollider").GetComponent<CheckGrounded>();
        _playerAnimator = transform.Find("ModelParent").GetComponent<Animator>();
        _playerAnimator = transform.Find("ModelParent").transform.Find("me_goober").GetComponent<Animator>();        
        _playerModelTransform = transform.Find("ModelParent");

        _states = new PlayerStateFactory(this);
        _currentState = _states.Idle();
        _currentState.EnterState();
    }

    void Update()
    {
        _currentState.UpdateState();
    }

    // Sends a message to the current state with the tag of a trigger
    public void MessageCurrentState(string tag)
    {
        _currentState.TakeMessage(tag);
    }
}

using UnityEngine;

// Class used to send information to the player state machine through animation events  
public class PlayerAnimationHandler : MonoBehaviour
{
    PlayerStateMachiene _playerStateMachiene;

    private void Start()
    {
        _playerStateMachiene = transform.parent.parent.GetComponent<PlayerStateMachiene>();
    }

    public void OnAnimationFinish()
    {
        _playerStateMachiene.AnimFinished = true;
    }

    public void OnLimbSwap()
    {
        _playerStateMachiene.OnLimbSwap = true;
    }
}
using System.Collections;
using UnityEngine;

public class PlayerEdgeClimbState : PlayerBaseState, IImportComponentToState
{
    private Transform _parentTransform;
    private float _currentRotationVelocity;
    private float _rotationTime = 0.1f;

    public PlayerEdgeClimbState(PlayerStateMachiene currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory) { }

    public override void CheckSwitchStates()
    {
        if (_ctx.AnimFinished)
            SwitchState(_factory.Idle());
    }

    public override void EnterState()
    {
        _ctx.PlayerAnimator.CrossFadeInFixedTime("Base Layer.ClimbOverEdge", 0.2f);
        Import();

        if (_parentTransform != null)
            _ctx.StartCoroutine(RotateToEdge());
    }

    public override void ExitState()
    {
        _parentTransform = null;

        Vector3 rootPosition = _ctx.transform.Find("ModelParent/me_goober/RootJNT").position;
        Vector3 footPosition = _ctx.transform.Find("ModelParent/me_goober/RootJNT/R_HipJNT/R_KneeJNT/R_AnkleJNT/R_FootJNT").position;

        Vector3 newPos = new Vector3(rootPosition.x, footPosition.y + _ctx.CharacterController.height / 2, rootPosition.z);

        // Disables the character controller to move the player to the correct location to avoid issues
        _ctx.CharacterController.enabled = false;
        _ctx.transform.position = newPos;
        _ctx.CharacterController.enabled = true;

        _ctx.PlayerAnimator.Play("Base Layer.anim_gooberIdle");
        _ctx.AnimFinished = false;
    }

    public override void TakeMessage(string message) { }

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public void Import()
    {
        if (_ctx.ImportInfo != null) // Avoid error on TriggerEdgeClimb, used for EdgeClimb
        {
            try 
            {
                _parentTransform = (Transform)_ctx.ImportInfo;
            }
            catch
            {
                Debug.Log("PlayerEdgeClimbState: Component is not a Transform");
                SwitchState(_factory.Idle());
            }
        }
    }

    // Coroutine handling rotating towards the targeted edge
    private IEnumerator RotateToEdge()
    {
        float targetYRotation = _parentTransform.localRotation.eulerAngles.y;
        float angle = 0f;

        while (!(_ctx.PlayerModelTransform.localRotation.eulerAngles.y < targetYRotation + 5 && _ctx.PlayerModelTransform.localRotation.eulerAngles.y > targetYRotation - 5))
        {
            angle = Mathf.SmoothDampAngle(_ctx.PlayerModelTransform.localRotation.eulerAngles.y, targetYRotation, ref _currentRotationVelocity, _rotationTime);
            _ctx.PlayerModelTransform.localRotation = Quaternion.Euler(0f, angle, 0f);
            yield return null;
        }

        _ctx.PlayerModelTransform.localRotation = Quaternion.Euler(0f, targetYRotation, 0f);
    }
}
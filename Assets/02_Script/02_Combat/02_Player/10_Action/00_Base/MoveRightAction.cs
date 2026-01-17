using UnityEngine;

public class MoveRightAction : ActionBase
{
    private float moveSpeed = 15f;

    protected override void OnStart()
    {
        player.StateComponent.ChangeState(ePlayerState.Move);
        player.ActionComponent.SetLookDir(1);
        player.ActionComponent.Play(ePlayerMotionType.Run, true);
    }

    protected override void OnKeyHolding()
    {
        player.ControllerComponent.transform.position += new Vector3(moveSpeed * Time.deltaTime, 0, 0);
    }

    protected override void OnKeyUp()
    {
        player.ControllerComponent.EndCurrentSkill();
    }

    protected override void OnEnd()
    {
        player.StateComponent.ChangeState(ePlayerState.Idle);
        player.ActionComponent.Play(ePlayerMotionType.Idle, true);
    }
}

using UnityEngine;

public class MoveRightAction : ActionBase
{
    private float moveSpeed = 15f;

    protected override void OnStart()
    {
        player.ActionComponent.SetLookDir(1);
        player.ActionComponent.Play(ePlayerMotionType.Run, true);
    }

    protected override void OnUpdate()
    {
        player.ControllerComponent.transform.position += new Vector3(moveSpeed * Time.deltaTime, 0, 0);
    }

    protected override void OnEnd()
    {
        player.ActionComponent.Play(ePlayerMotionType.Idle, true);
    }
}

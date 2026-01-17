
using UnityEngine;

public abstract class SkillBase : ActionBase
{
    protected float cooldownTime;
    protected float lastUsedTime;

    protected sealed override void OnStart()
    {
        player.EventBus.ConnectEvent<OnAnimatorEvent>(OnAnimatorEvent);
        player.StateComponent.ChangeState(ePlayerState.Skill);
        OnStartSkill();
        lastUsedTime = Time.time;
    }

    protected virtual void OnStartSkill()
    {
        
    }

    protected sealed override void OnEnd()
    {
        player.EventBus.DisconnectEvent<OnAnimatorEvent>(OnAnimatorEvent);
        player.StateComponent.ChangeState(ePlayerState.Idle);
        player.ActionComponent.Play(ePlayerMotionType.Idle, true, 1, 0);
        OnEndSkill();
    }

    protected virtual void OnEndSkill()
    {
        
    }

    protected virtual void OnAnimatorEvent(OnAnimatorEvent evt)
    {
        
    }

    public override bool CanStart()
    {
        return (Time.time >= lastUsedTime + cooldownTime) && (player.StateComponent.State != ePlayerState.Skill);
    }
}
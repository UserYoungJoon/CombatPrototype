using UnityEngine;

public abstract class SkillBase
{
    protected PlayerController playerController;
    protected PlayerAnimation playerAnimation;
    protected EventBus eventBus;

    public void Init(PlayerController _playerController, PlayerAnimation _playerAnimation, EventBus _eventBus)
    {
        this.playerController = _playerController;
        this.playerAnimation = _playerAnimation;
        this.eventBus = _eventBus;
        OnInit();
    }

    public virtual void OnInit() {}
    public virtual void OnStart() {}
    public virtual void OnUpdate() {}
    public virtual void OnEnd() {}
    public virtual void OnAborted() {}
}
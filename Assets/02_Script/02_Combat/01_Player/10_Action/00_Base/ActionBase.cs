using UnityEngine;

public abstract class ActionBase
{
    protected PlayerController playerController;
    protected PlayerAction playerAnimation;
    protected EventBus eventBus;

    public void Init(PlayerController _playerController, PlayerAction _playerAnimation, EventBus _eventBus)
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
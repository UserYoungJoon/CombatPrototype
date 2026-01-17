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

    public void Start()
    {
        OnStart();
    }

    public void Update()
    {
        OnUpdate();
    }

    public void End()
    {
        OnEnd();
    }

    public void Aborted()
    {
        OnAborted();
    }

    protected virtual void OnInit() {}
    protected virtual void OnStart() {}
    protected virtual void OnUpdate() {}
    protected virtual void OnEnd() {}
    protected virtual void OnAborted() {}
    
    public virtual bool CanStart() { return true; }
}
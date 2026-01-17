using UnityEngine;

public abstract class ActionBase
{
    protected PlayerUnit player;

    public void Init(PlayerUnit player)
    {
        this.player = player;
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
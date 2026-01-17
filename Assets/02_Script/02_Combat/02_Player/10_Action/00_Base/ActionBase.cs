using UnityEngine;

public abstract class ActionBase
{
    protected PlayerUnit player;

    private bool isRunning = false;
    public bool IsRunning => isRunning;

    public void Init(PlayerUnit player)
    {
        this.player = player;
        OnInit();
    }

    public void Start()
    {
        isRunning = true;
        OnStart();
    }

    public void Update()
    {
        OnUpdate();
    }

    public void End()
    {
        isRunning = false;
        OnEnd();
    }

    public void Aborted()
    {
        isRunning = false;
        OnAborted();
    }

    protected virtual void OnInit() {}
    protected virtual void OnStart() {}
    protected virtual void OnUpdate() {}
    protected virtual void OnEnd() {}
    protected virtual void OnAborted() {}
    
    public virtual bool CanStart() { return true; }
}
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

    public void KeyHolding()
    {
        OnKeyHolding();
    }

    public void KeyUp()
    {
        OnKeyUp();
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
    protected virtual void OnKeyHolding() {}
    protected virtual void OnKeyUp() {}
    protected virtual void OnEnd() {}
    protected virtual void OnAborted() {}
    
    public virtual bool CanStart() { return true; }
}
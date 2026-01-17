
public readonly struct OnAnimatorEvent : IGameEvent
{
    public readonly string EventName;
    
    public OnAnimatorEvent(string eventName)
    {
        this.EventName = eventName;
    }
}
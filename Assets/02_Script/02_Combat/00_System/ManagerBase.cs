using UnityEngine;

[RequireComponent(typeof(EventBus))]
public abstract class ManagerBase : MonoBehaviour
{
    private EventBus eventBus;
    public EventBus EventBus => eventBus;
    
    public void Init()
    {
        eventBus = GetComponent<EventBus>();
        OnInit();
    }

    protected virtual void OnInit()
    {
        
    }
}
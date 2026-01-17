
using UnityEngine;

public class PlayerUnit : Unit
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerAction playerAction;
    [SerializeField] private PlayerState playerState;
    [SerializeField] private EventBus eventBus;
    public PlayerController ControllerComponent => playerController;
    public PlayerAction ActionComponent => playerAction;
    public PlayerState StateComponent => playerState;
    public EventBus EventBus => eventBus;

    [SerializeField] private Transform attackPoint;
    public Transform AttackPoint => attackPoint;

    private void Awake()
    {
        playerController.Init();
        playerAction.Init();
        playerState.Init();
    }
}

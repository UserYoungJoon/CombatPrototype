
using UnityEngine;

public class PlayerUnit : Unit
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerAction playerAction;
    [SerializeField] private EventBus eventBus;

    private ePlayerState state = ePlayerState.Idle;
    public ePlayerState State => state;

}

public enum ePlayerState
{
    Idle,
    Move,
    Jump,
    Attack,
    Hit
}
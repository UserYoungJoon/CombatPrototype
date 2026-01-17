
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerAction playerAction;
    [SerializeField] private EventBus eventBus;

}

public enum ePlayerState
{
    Idle,
    Move,
    Jump,
    Attack
}
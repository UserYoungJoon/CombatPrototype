
using UnityEngine;

[RequireComponent(typeof(PlayerAction))]
public class PlayerState : MonoBehaviour
{
    [SerializeField] private PlayerAction playerAction;
    private ePlayerState state = ePlayerState.Idle;
    public ePlayerState State => state;

    public void Init()
    {
        playerAction = GetComponent<PlayerAction>();
    }

    public void ChangeState(ePlayerState newState)
    {
        if (state == newState) return;

        var oldState = state;
        state = newState;
        OnStateChanged(oldState, newState);
    }

    private void OnStateChanged(ePlayerState oldState, ePlayerState newState)
    {
        // Handle state transition logic here if needed
        switch (newState)
        {
            case ePlayerState.Idle:
                playerAction.Play(ePlayerMotionType.Idle, true);
                break;
            case ePlayerState.Move:
                break;
            case ePlayerState.Jump:
                break;
            case ePlayerState.SkillCanMove:
                break;
            case ePlayerState.Skill:
                break;
            case ePlayerState.Hit:
                break;
        }
    }
}
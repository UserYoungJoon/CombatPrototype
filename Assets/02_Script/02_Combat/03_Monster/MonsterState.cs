using UnityEngine;

public enum eMonsterState
{
    Idle,
    Hit
}

public class MonsterState : MonoBehaviour
{
    [SerializeField] private eMonsterState state = eMonsterState.Idle;
    public eMonsterState State => state;

    public void Init()
    {
    }

    public void ChangeState(eMonsterState newState)
    {
        if (state == newState) return;
        state = newState;
    }
}

using UnityEngine;

public class MonsterUnit : Unit
{
    [SerializeField] private MonsterAction monsterAction;
    [SerializeField] private MonsterState monsterState;

    public MonsterAction ActionComponent => monsterAction;
    public MonsterState StateComponent => monsterState;

    private void Awake()
    {
        monsterAction.Init();
        monsterState.Init();
        monsterAction.PlayIdle();
    }

    protected override void OnAttacked(Unit attacker, int damage)
    {
        monsterState.ChangeState(eMonsterState.Hit);
        monsterAction.Play(eMonsterMotionType.Damage1);
    }

    public override void Die()
    {
        monsterAction.Play(eMonsterMotionType.Die);
    }
}

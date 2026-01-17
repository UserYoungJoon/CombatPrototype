
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private int currentHP;
    [SerializeField] private int attackPower;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float attackSpeed;
    public int CurrentHP => currentHP;
    public int AttackPower => attackPower;
    public float MoveSpeed => moveSpeed;
    public float AttackSpeed => attackSpeed;

    public bool IsDead => currentHP <= 0;

    public void AttackedBy(Unit attacker, int damage)
    {
        if (IsDead) return;

        currentHP -= damage;
        OnAttacked(attacker, damage);

        if (currentHP <= 0)
        {
            Die();
            GameManager.Instance.EventBus.SendEvent(new OnUnitDie(attacker, this));
        }
    }

    protected virtual void OnAttacked(Unit attacker, int damage)
    {
        
    }

    public virtual void Die()
    {
        
    }
}
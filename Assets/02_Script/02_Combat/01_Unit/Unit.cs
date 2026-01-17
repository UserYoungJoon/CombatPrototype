
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
}
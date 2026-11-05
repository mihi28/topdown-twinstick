using System.Buffers.Text;
using UnityEngine;

public enum EnemyAttackType{ Melee, Ranged, Shielder, Kamikaze}

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;

    [Header("Base Stats")]
    [SerializeField] public float maxHealth;
    [SerializeField] public float armour;
    [SerializeField] public float speed;
    [SerializeField] public float damage;
    [SerializeField] public bool isElite;

    [Header("Attacking Behaviour")]
    [SerializeField] public EnemyAttackSO attackData;
    [SerializeField] public EnemyAttackType enemyAttackType;
    [SerializeField] public float attackRange;
    [SerializeField] public float attackWindupTime; //time before attack triggers
    [SerializeField] public float attackDuration; //time for attack to resolve
    [SerializeField] public float attackCooldown; //time before returning to chase
    [SerializeField] public float timeBetweenAttacks; //time before being able to launch another attack
    
    [Header("Visual")]
    [SerializeField] public Material material;
    [SerializeField] public float scale;
}

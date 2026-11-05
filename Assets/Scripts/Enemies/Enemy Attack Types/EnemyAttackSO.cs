using UnityEngine;

public abstract class EnemyAttackSO : ScriptableObject
{
    public abstract void ExecuteAttack(Transform attacker, Transform target, EnemyData data);
}

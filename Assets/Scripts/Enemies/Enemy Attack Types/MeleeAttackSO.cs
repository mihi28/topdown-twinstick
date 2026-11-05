using System;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeAttack", menuName = "Scriptable Objects/MeleeAttack")]
public class MeleeAttackSO : EnemyAttackSO
{
    [Header("Settings")]
    [SerializeField] public float meleeConeAngle;
    [SerializeField] public LayerMask targetLayer;
    public override void ExecuteAttack(Transform attacker, Transform target, EnemyData data)
    {
        Collider[] hits = Physics.OverlapSphere(attacker.position, data.attackRange, targetLayer);

        foreach(Collider hit in hits)
        {
            Vector3 directionToTarget = (hit.transform.position - attacker.position).normalized;
            float angleToTarget = Vector3.Angle(attacker.forward, directionToTarget);

            if (angleToTarget < (meleeConeAngle/ 2f))
            {
                Debug.Log("playerHit by melee");
                // HIT! 
                // hit.GetComponent<PlayerHealth>().TakeDamage(stats.attackDamage);
            }
        }

        //throw new System.NotImplementedException();
    }
}

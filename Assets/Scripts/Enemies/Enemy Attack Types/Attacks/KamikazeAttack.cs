using UnityEngine;

[CreateAssetMenu(fileName = "KamikazeAttack", menuName = "Scriptable Objects/KamikazeAttack")]
public class KamikazeAttack : EnemyAttackSO
{
    public override void ExecuteAttack(Transform attacker, Transform target, EnemyData data)
    {
        //it just dies so

        if(attacker.TryGetComponent(out EnemyHandler handler))
        {
            handler.Die();
        }

        //throw new System.NotImplementedException();
    }
}

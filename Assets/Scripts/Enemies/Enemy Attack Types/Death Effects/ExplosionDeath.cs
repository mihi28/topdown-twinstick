using UnityEngine;

[CreateAssetMenu(fileName = "ExplosionDeath", menuName = "Scriptable Objects/ExplosionDeath")]
public class ExplosionDeath : AbstractDeathRattleSO
{
    [Header("Settings")]
    [SerializeField] float explosionPlayerDamage;
    [SerializeField] float explosionEnemyDamage;
    [SerializeField] float explosionRadius;
    [SerializeField] LayerMask targetLayerMask;

    public override void ExecuteDeathRattle(Transform attacker)
    {
        //spawn a sphere, deals damage to all inside the sphere

        Collider[] hits = Physics.OverlapSphere(attacker.position, explosionRadius, targetLayerMask);
        

        foreach(Collider hit in hits)
        {
            // ignore ourselves so we don't blow up our own dead body
            if (hit.transform == attacker || hit.gameObject == attacker.gameObject) 
                continue;

            //comb through hits, damage targets based on their tag
            //enemies should take less than the player
            if(hit.TryGetComponent(out IDamageable damageableTarget))
            {
                DamageInfo damageInfo = new DamageInfo{};

                if (hit.CompareTag("Enemy"))
                {
                    Debug.Log("enemy took damage from explosion");
                    damageInfo.amount = explosionEnemyDamage;
                }
                
                else if (hit.CompareTag("Player"))
                {
                    Debug.Log("player took damage from explosion");
                    damageInfo.amount = explosionPlayerDamage;
                }
                
                damageableTarget.takeDamage(damageInfo);
            }
        }
        //throw new System.NotImplementedException();
    }
}

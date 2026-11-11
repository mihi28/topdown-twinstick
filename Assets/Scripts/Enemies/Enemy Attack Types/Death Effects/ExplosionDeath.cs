using UnityEngine;

[CreateAssetMenu(fileName = "ExplosionDeath", menuName = "Scriptable Objects/ExplosionDeath")]
public class ExplosionDeath : AbstractDeathRattleSO
{
    [Header("Settings")]
    [SerializeField] float explosionDamage;
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

            if(hit.TryGetComponent(out IDamageable damageableTarget))
            {
                Debug.Log("took damage from explosion");
                DamageInfo damageInfo = new DamageInfo{
                    amount = explosionDamage,
                    isCritical = false
                };
                
                damageableTarget.takeDamage(damageInfo);
            }
        }
        //throw new System.NotImplementedException();
    }
}

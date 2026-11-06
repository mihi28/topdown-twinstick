using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "BulletAtackSO", menuName = "Scriptable Objects/BulletAtackSO")]
public class BulletAtackSO : EnemyAttackSO
{
    [Header("Settings")]
    [SerializeField] private float bulletVelocity;
    [SerializeField] private float bulletLifetime;
    [SerializeField] private Vector3 originOffset;
    [SerializeField] private Material enemyBulletMaterial;

    public override void ExecuteAttack(Transform attacker, Transform target, EnemyData data)
    {
        //get a bullet from the pool
        GameObject bullet = BulletPool.Instance.GetBullet();

        //assign direction and origin of shot
        Vector3 shootingDirection = attacker.forward;
        Vector3 origin = attacker.transform.position + originOffset;

   /*     if (float.IsNaN(origin.x) || float.IsNaN(shootingDirection.x))
{
    Debug.LogError("MATH CORRUPTION DETECTED! Origin: " + origin + " Direction: " + shootingDirection);
    return; // Stop before it crashes the Canvas!
}*/

        bullet.transform.position = origin;
        bullet.transform.forward = shootingDirection;

        //intialize the bullet
        bullet.GetComponent<BulletLogic>().InitializeBullet(
            "Enemy",
            enemyBulletMaterial,
            bulletVelocity,
            bulletLifetime,
            data.damage,
            0f,
            0f
        );

        //throw new System.NotImplementedException();
    }
}

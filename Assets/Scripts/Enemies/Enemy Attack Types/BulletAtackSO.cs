
using UnityEngine;

[CreateAssetMenu(fileName = "BulletAtackSO", menuName = "Scriptable Objects/BulletAtackSO")]
public class BulletAtackSO : EnemyAttackSO
{
    [Header("Settings")]
    [SerializeField] private float bulletVelocity;
    [SerializeField] private float bulletLifetime;
    [SerializeField] private float playerPredictionMultiplier;
    [SerializeField] private Material enemyBulletMaterial;

    public override void ExecuteAttack(Transform attacker, Transform target, EnemyData data)
    {
        //get a bullet from the pool
        GameObject bullet = BulletPool.Instance.GetBullet();

        //get the velocity of the players rb to try and predict it a littlebit
        Vector3 playerMovementDirection = target.GetComponent<Rigidbody>().linearVelocity;

        //get the distance so it's dynamic, the further the player the more it predicts
        float distanceToPlayer = Vector3.Distance(attacker.position, target.position);

        Vector3 predictionOffset = playerMovementDirection.normalized * distanceToPlayer * playerPredictionMultiplier;
        Vector3 predictionPoint = target.position + predictionOffset;

        //assign direction and origin of shot
        Vector3 shootingDirection = (predictionPoint - attacker.position).normalized;
        Vector3 origin = attacker.position;

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

using UnityEngine;

[CreateAssetMenu(fileName = "LobbingAttack", menuName = "Scriptable Objects/LobbingAttack")]
public class LobbingAttack : EnemyAttackSO
{
    [Header("Settings")]
    [SerializeField] private float poolRadius;
    [SerializeField] private float playerDamage;
    [SerializeField] private float enemyDamage;
    [SerializeField] private float poolDuration;
    [SerializeField] private float tickDamageFrequency;
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private float playerPredictionMultiplier;

    public override void ExecuteAttack(Transform attacker, Transform target, EnemyData data)
    {
        //calculate target where the pool should spawn
        //same logic as shooting a bullet, but predict the point behind the player instead

        //get the velocity of the players rb to try and predict it a littlebit
        Vector3 playerMovementDirection = target.GetComponent<Rigidbody>().linearVelocity;

        //get the distance so it's dynamic, the further the player the more it predicts
        float distanceToPlayer = Vector3.Distance(attacker.position, target.position);

        Vector3 predictionOffset = -playerMovementDirection.normalized * distanceToPlayer * playerPredictionMultiplier;
        Vector3 predictionPoint = target.position + predictionOffset; //where the pool should spawn
        predictionPoint.y = 0f;

        //play throwing animation

        //get pool from pool and place it where it should be
        GameObject acidPool = AcidPoolPool.Instance.GetAcidPool();
        acidPool.transform.position = predictionPoint;
        //scale the prefab accordingly
        acidPool.transform.localScale = new Vector3(poolRadius, acidPool.transform.localScale.y, poolRadius);

        //calculate how much damage it should deal every tick
        float tickPlayerDamage = playerDamage / (poolDuration / tickDamageFrequency);
        float tickEnemyDamage = enemyDamage / (poolDuration / tickDamageFrequency);

        //initialize and spawn the pool
        acidPool.GetComponent<AcidPoolLogic>().InitializeAcidPool(
            poolRadius,
            poolDuration,
            tickDamageFrequency,
            tickPlayerDamage,
            tickEnemyDamage,
            targetLayerMask
        );

        //throw new System.NotImplementedException();
    }
}

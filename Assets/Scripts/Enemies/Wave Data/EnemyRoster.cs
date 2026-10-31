using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawnCost
{
    public EnemyData enemyData;
    public int cost;
};


[CreateAssetMenu(fileName = "WaveData", menuName = "Scriptable Objects/EnemyRoster")]
public class EnemyRoster : ScriptableObject
{
    //basically just a way to build different enemy roster, it s just a list
    public List<EnemySpawnCost> availableEnemies;
}

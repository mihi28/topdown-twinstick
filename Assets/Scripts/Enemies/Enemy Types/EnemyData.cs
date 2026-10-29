using System.Buffers.Text;
using UnityEngine;

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
    
    [Header("Visual")]
    [SerializeField] public Material material;
    [SerializeField] public float scale;
}

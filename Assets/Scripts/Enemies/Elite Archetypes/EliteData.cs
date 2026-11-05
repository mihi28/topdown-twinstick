using UnityEngine;

[CreateAssetMenu(fileName = "EliteData", menuName = "Scriptable Objects/EliteData")]
public class EliteData : ScriptableObject
{
    public string eliteType;

    [Header("Stats")]
    public float healthMultiplier;
    public float armourMultiplier;
    public float damageMultiplier;
    public float speedMultiplier;

    [Header("AI Behaviour Changes")]
    public float attackRangeMultiplier;
    //public float attackDurationMultiplier;
    public float timeBetweenAttacksMultiplier;

    [Header("Visual")]
    public Material material;
    //public Color color;
    public float scale;
}

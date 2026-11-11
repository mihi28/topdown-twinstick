using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "Scriptable Objects/WaveData")]
public class WaveData : ScriptableObject
{
    //a scriptable object special for different types of wave
    //for now it contains which wave number, and the roster of enemies spawned
    // in the future can change spawn delays, wave timers, maybe even spawn intensity curve, elite spawning chance
    //wave number 0 means default wave

    [Header("Wave Settings")]
    public int waveNumber;
    public EnemyRoster roster;
    public List<EliteData> availableElites;
    public float eliteSpawnChance;
    //public float eliteUpgradeCost;
}

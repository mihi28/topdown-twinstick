using UnityEngine;

public abstract class AbstractDeathRattleSO : ScriptableObject
{
    public abstract void ExecuteDeathRattle(Transform attacker);
}

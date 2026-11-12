using UnityEngine;

public struct DamageInfo
{
    public float amount;
    public bool ignoreArmour;
    public bool isCritical;
}


public interface IDamageable
{
    public void takeDamage(DamageInfo info);
}
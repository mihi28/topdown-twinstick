using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] public float maxHealth = 0f;
    [SerializeField] public float armour = 0f;
    [SerializeField] public float speed = 0f;
    [SerializeField] public float damage = 0f;
    [SerializeField] public float critChance = 0f;
    [SerializeField] public float critMultiplier = 0f;
    [SerializeField] public float fireRate = 0f;
    [SerializeField] public float range = 0f; //basically lifetime of the bullets
    [SerializeField] public float bulletVelocity = 0f;

}

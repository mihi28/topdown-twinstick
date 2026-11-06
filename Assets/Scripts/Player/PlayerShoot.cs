using System;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{

    [Header("Stats")]
    [SerializeField] private float fireRate;
    [SerializeField] private float fireCd;

    [Header("Components")]
    [SerializeField] private Material playerBulletMaterial;
    [SerializeField] private InputAction shootAction;
    [SerializeField] private PlayerAim _playerAimScript;
    [SerializeField] private PlayerStats _stats;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shootAction = InputSystem.actions.FindAction("Player/Attack");

        fireRate = _stats.fireRate;
    }

    // Update is called once per frame
    void Update()
    {
        if (shootAction.IsPressed() && fireCd <= 0f)
        {
            spawnBullet(BulletPool.Instance.GetBullet());
        }

        fireCd = Mathf.Max(0, fireCd - Time.deltaTime);
    }

    public void spawnBullet(GameObject bullet)
    {
        fireCd = fireRate;
        

        Vector3 shootingDirection = _playerAimScript.currentAimDirection;

        bullet.transform.position = transform.position;
        bullet.transform.forward = shootingDirection;

        BulletLogic bulletScript = bullet.GetComponent<BulletLogic>();
        bulletScript.InitializeBullet(
            "Player",
            playerBulletMaterial,
            _stats.bulletVelocity, 
            //player.GetComponent<PlayerMovement>().getPlayerVelocity(),
            _stats.range, 
            _stats.damage, 
            _stats.critChance, 
            _stats.critMultiplier
        );
    }

}

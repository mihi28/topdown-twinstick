using System;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{

    [Header("Stats")]
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float fireCd = 0f;

    [Header("Components")]
    [SerializeField] private InputAction shootAction;
    [SerializeField] private PlayerAim _playerAimScript;
    [SerializeField] private GameObject origin;
    [SerializeField] private GameObject bullet;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shootAction = InputSystem.actions.FindAction("Player/Attack");

        if(origin == null || bullet == null)
        {
            return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (shootAction.IsPressed() && fireCd <= 0f)
        {
            spawnBullet(bullet);
        }

        if(fireCd > 0f)
            fireCd -= Time.deltaTime;
    }

    public void spawnBullet(GameObject bullet)
    {
        fireCd = fireRate;
        
        Vector3 shootingDirection = _playerAimScript.currentAimDirection;

        GameObject newBullet = Instantiate(bullet, transform.position, Quaternion.identity);
        newBullet.transform.forward = shootingDirection;
    }

}


using System;
using FishNet.Object;
using UnityEngine;

public class BulletNB : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb;

    private Vector3 velocity;
    private bool initialized = false;
    /*public void Setup(Vector3 direction, float speed)
    {
        rb.AddForce(direction * speed, ForceMode.Impulse);
        
        Invoke("DespawnBullet", 2f);
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServerStarted) return;
        DespawnBullet();
    }

    [Server]
    private void DespawnBullet()
    {
        Despawn();
    }*/
    public void Setup(Vector3 direction, float speed)
    {
        velocity = direction * speed;
        if (IsServerStarted)
        {
            rb.linearVelocity = velocity;

            InitializeClientsBullet(velocity);
        }

        initialized = true;
        Invoke("DespawnBullet", 2f);
    }

    [ObserversRpc]
    private void InitializeClientsBullet(Vector3 vel)
    {
        velocity = vel;
        initialized = true;
    }

    private void FixedUpdate()
    {
        if (!IsServerInitialized && initialized)
        {
            rb.linearVelocity = velocity;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServerInitialized) return;
        DespawnBullet();
    }

    [Server]
    private void DespawnBullet()
    {
        Despawn();
    }
}

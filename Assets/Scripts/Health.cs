
using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public abstract class Health : NetworkBehaviour
{
    protected int MaxHealth = 100;
    public readonly SyncVar<int> currentHealth = new SyncVar<int>();

    private void Start()
    {
        currentHealth.Value = MaxHealth;
    }

    public void TakeDamage(int damage)
    
    {
        currentHealth.Value -= damage;
        currentHealth.Value = Mathf.Clamp(currentHealth.Value, 0, MaxHealth);
        Debug.Log($"Player {gameObject.name } was hit and has {currentHealth.Value} health ");
        CheckDeath();
    }

    private void CheckDeath()
    {
        if (currentHealth.Value <= 0)
        {
            Die();
        }
    }

    protected abstract void Die();

}

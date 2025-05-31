
using System;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public abstract class Health : NetworkBehaviour
{
    protected int MaxHealth = 100;
    public readonly SyncVar<int> currentHealth = new SyncVar<int>();

    public override void OnStartClient()
    {
        base.OnStartClient();
        currentHealth.OnChange += HealthChanged;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        currentHealth.Value = MaxHealth;
    }
    
    private void OnDestroy()
    {
        currentHealth.OnChange -= HealthChanged;
    }

    protected virtual void HealthChanged(int prev, int next, bool asserver)
    {
        
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

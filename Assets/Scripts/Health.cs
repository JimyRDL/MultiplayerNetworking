
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


    public void TakeDamage(int damage, GameObject playerShooter)
    {
        currentHealth.Value -= damage;
        currentHealth.Value = Mathf.Clamp(currentHealth.Value, 0, MaxHealth);
        CheckDeath(playerShooter);
    }

    private void CheckDeath(GameObject playerShooter)
    {
        if (currentHealth.Value <= 0)
        {
            Die(playerShooter);
        }
    }

    protected abstract void Die(GameObject playerShooter);

}

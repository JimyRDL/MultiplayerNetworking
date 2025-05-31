using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthNB : Health
{
    [SerializeField] private GameObject healthParent;
    [SerializeField] private Image healthBar;

    public override void OnStartClient()
    {
        base.OnStartClient();
        healthParent.SetActive(IsOwner);
    }

    protected override void HealthChanged(int prev, int next, bool asserver)
    {
        base.HealthChanged(prev, next, asserver);
        healthBar.fillAmount = next / (float)MaxHealth;
    }

    protected override void Die()
    {
        Destroy(this.gameObject);
    }
}
using System;
using FishNet.Connection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthNB : Health
{
    [SerializeField] private GameObject healthParent;
    [SerializeField] private Image healthBar;

    
    private PlayerTeamManager playerTeamManager;
    public override void OnStartClient()
    {
        base.OnStartClient();
        healthParent.SetActive(IsOwner);
    }

    private void Awake()
    {
        playerTeamManager = GetComponent<PlayerTeamManager>();
    }

    protected override void HealthChanged(int prev, int next, bool asserver)
    {
        base.HealthChanged(prev, next, asserver);
        healthBar.fillAmount = next / (float)MaxHealth;
    }

    protected override void Die()
    {
        DieEvent evt = new DieEvent();
        evt.player = this.gameObject;
        evt.team = playerTeamManager.team.Value;
        evt.connection = Owner;
        EventManager.Broadcast(evt);
    }
}
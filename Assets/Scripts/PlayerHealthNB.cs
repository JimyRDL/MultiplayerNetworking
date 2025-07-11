using System;
using DefaultNamespace;
using FishNet.Connection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthNB : Health
{
    [SerializeField] private GameObject healthParent;
    [SerializeField] private Image healthBar;
    [SerializeField] private Image healthBarTopPlayer;
    
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
        healthBarTopPlayer.fillAmount = next / (float)MaxHealth;
    }

    protected override void Die(GameObject playerShooter)
    {
        PlayerSessionNB playerSession = GetComponent<PlayerControllerNB>().PlayerSession;
        DieEvent evt = new DieEvent();
        evt.team = playerTeamManager.team.Value;
        evt.playerDead = this.gameObject;
        evt.playerShooter = playerShooter;
        evt.connection = Owner;
        EventManager.Broadcast(evt);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using TMPro;
using UnityEngine;

public class GameStarterManager : NetworkBehaviour
{
    private int countdown = 4;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private GameObject countdownParent;
    [SerializeField] private Transform[] spawnPoints;
    private int spawnIndex = 0;

    private void OnEnable()
    {
        EventManager.AddListener<StartCountdownEvent>(StartCountdown);
    }
    private void OnDisable()
    {
        EventManager.RemoveListener<StartCountdownEvent>(StartCountdown);
    }

 

    public override void OnStartClient()
    {
        base.OnStartClient();
        UpdateCountdownClient(3);
    }

    [ObserversRpc]
    private void UpdateCountdownClient(int number)
    {
        countdownText.text = number.ToString();
    }

    [Server]
    private void StartCountdown(StartCountdownEvent evt)
    {
        ActivateBackground();
        StartCoroutine(CoroutineCountdown());
    }

    [ObserversRpc]
    private void ActivateBackground()
    {
        countdownParent.SetActive(true);
    }
    
    private IEnumerator CoroutineCountdown()
    {
        while (countdown > 0)
        {
            countdown--;
            UpdateCountdownClient(countdown);
            yield return new WaitForSeconds(1f);
        }
        FinishedCountdown();
    }

    public Vector3 GetNextSpawnPoint()
    {
        Vector3 pos = spawnPoints[spawnIndex % spawnPoints.Length].position;
        spawnIndex++;
        return pos;
    }
    
    [ObserversRpc]
    private void FinishedCountdown()
    {
        countdownParent.SetActive(false);
        EventManager.Broadcast(Events.countdownFinishedEvent);
    }
}

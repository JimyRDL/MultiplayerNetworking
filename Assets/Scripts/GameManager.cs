using System;
using FishNet.Connection;
using UnityEngine;
using FishNet.Object;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
     private UIManager uIManager;

     private int blueTeamScore;
     private int redTeamScore;
     private int maxTeamScore = 10;
     [Header("Project References")]
     [SerializeField]
     private GameObject playerPrefab;
     public enum Teams
     {
          None,
          Blue,
          Red
     }

     public enum GameState
     {
          ConnectionLobby,
          CharacterSelectionLobby,
          StartingGame,
          Game
     }

     public static GameState actualState;

     private void OnEnable()
     {
          EventManager.AddListener<DieEvent>(PlayerDied);
          EventManager.AddListener<CountdownFinishedEvent>(FinishedCountdown);
     }
     private void OnDisable()
     {
          EventManager.RemoveListener<DieEvent>(PlayerDied);
          EventManager.RemoveListener<CountdownFinishedEvent>(FinishedCountdown);
     }
     private void Start()
     {
          actualState = GameState.StartingGame;
     }
     
     
     private void FinishedCountdown(CountdownFinishedEvent evt)
     {
          ChangeState(GameState.Game);
          ActivateScoresUI();
     }

    

     [Server]
     public void ChangeState(GameState newState)
     {
          actualState = newState;
          ReceiveChangeState(newState);
     }

     [ObserversRpc]
     private void ReceiveChangeState(GameState newState)
     {          
          actualState = newState;
     }
     

     private void PlayerDied(DieEvent evt)
     {
          RespawnPlayer(evt.player, evt.connection);
          UpdateScores(evt.team);
     }
     
     private void UpdateScores(Teams team)
     {
          if (team == Teams.Blue)
          {
               redTeamScore++;
          } else if (team == Teams.Red)
          {
               blueTeamScore++;
          }
          Debug.Log($"BlueTeamScore: {blueTeamScore}, RedTeamScore: {redTeamScore}, MaxTeamScore: {maxTeamScore}");
          CheckScores();
          UpdateScoresUI(blueTeamScore, redTeamScore, maxTeamScore);
     }

     private void CheckScores()
     {
          if (blueTeamScore >= maxTeamScore)
          {
               Debug.Log("Bluewon");
          } else if (redTeamScore >= maxTeamScore)
          {
               Debug.Log("Redwon");
          }
     }

     [ObserversRpc]
     private void ActivateScoresUI()
     {
          UIManager.Instance.ActivateScoresUI();
          UpdateScoresUI(blueTeamScore, redTeamScore, maxTeamScore);
     }
     
     [ObserversRpc]
     private void UpdateScoresUI(int BTS, int RTS, int MTS)
     {
          UIManager.Instance.UpdateScoresUI(BTS, RTS, MTS);
     }

     private void RespawnPlayer(GameObject playerToKill, NetworkConnection connection)
     {
          GameObject previousWeapon = playerToKill.GetComponent<PlayerWeaponManagerNB>().ActualWeaponPrefab;
          Despawn(playerToKill);
          GameObject playerGO = Instantiate(playerPrefab);
          Spawn(playerGO, connection);
          PlayerWeaponManagerNB weaponManager = playerGO.GetComponent<PlayerWeaponManagerNB>();
          weaponManager.SetupWeapon(previousWeapon, connection);
     }
}

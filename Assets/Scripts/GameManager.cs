using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DefaultNamespace;
using FishNet.Connection;
using FishNet.Managing.Scened;
using UnityEngine;
using FishNet.Object;

public class GameManager : NetworkBehaviour
{
     private UIManager uIManager;

     private int blueTeamScore;
     private int redTeamScore;
     private int maxTeamScore = 10;
     [Header("Scene References")]
     [SerializeField] private Transform[] spawnPoints;
     [Header("Project References")]
     [SerializeField]
     private GameObject playerPrefab;
     public enum Teams
     {
          None,
          Blue,
          Red
     }

     public enum GameModes
     {
          TeamDeathmatch,
          FreeForAll,
          None
     }
     
     public GameModes actualMode;

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
          ShowKilledUI(evt.playerDead, evt.playerShooter);
          UpdateScores(evt.playerDead.GetComponent<PlayerControllerNB>().PlayerSession.Team.Value);
          RespawnPlayer(evt.playerDead, evt.connection);
     }
     
     [ObserversRpc]
     private void ShowKilledUI(GameObject playerDead, GameObject playerShooter)
     {
          UIManager.Instance.UpdateKillFeed(
               playerDead.GetComponent<PlayerControllerNB>().PlayerSession.PlayerName.Value, 
               playerShooter.GetComponent<PlayerControllerNB>().PlayerSession.PlayerName.Value, 
               playerDead.GetComponent<PlayerControllerNB>().PlayerSession.Team.Value,
               playerShooter.GetComponent<PlayerControllerNB>().PlayerSession.Team.Value);
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
          CheckScores();
          UpdateScoresUI(blueTeamScore, redTeamScore, maxTeamScore);
     }

     private void CheckScores()
     {
          bool someoneWon = false;
          if (blueTeamScore >= maxTeamScore)
          {
               KeepTrackScores.teamWon = 0;
               someoneWon = true;
          } else if (redTeamScore >= maxTeamScore)
          {
               KeepTrackScores.teamWon = 1;
               someoneWon = true;
          }

          if (!someoneWon)
               return;
          StartCoroutine(ChangeScene());
          
     }

     private IEnumerator ChangeScene()
     {
          yield return new WaitForSeconds(1f);
          SceneLoadData scene = new SceneLoadData("EndScene")
          {
               ReplaceScenes = ReplaceOption.All
          };
          SceneManager.UnloadGlobalScenes(new SceneUnloadData("MainScene"));
          SceneManager.LoadGlobalScenes(scene);
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

     
     // Feedback: Respawn everything automatically might create problems, create a delay, with a coroutine
     private void RespawnPlayer(GameObject playerToKill, NetworkConnection connection)
     {
          GameObject previousWeapon = playerToKill.GetComponent<PlayerWeaponManagerNB>().ActualWeaponPrefab;
          Teams previousTeam = playerToKill.GetComponent<PlayerTeamManager>().team.Value;
          PlayerControllerNB oldPlayerController = playerToKill.GetComponent<PlayerControllerNB>();
          PlayerSessionNB oldSession = oldPlayerController.PlayerSession;
          Despawn(playerToKill);
          
          
          
          GameObject playerGO = Instantiate(playerPrefab);
          playerGO.GetComponent<PlayerControllerNB>().InitializeSession(oldSession);
          playerGO.SetActive(false);
          playerGO.transform.position = GetSpawnPointFree().position;
          playerGO.SetActive(true);
          Spawn(playerGO, connection);
          playerGO.GetComponent<PlayerTeamManager>().team.Value = previousTeam;
          PlayerWeaponManagerNB weaponManager = playerGO.GetComponent<PlayerWeaponManagerNB>();
          weaponManager.SetupWeapon(previousWeapon, connection);
     }    
     [ObserversRpc]
     private void ChangeNameObserverRPC(GameObject player, string newName)
     {
          player.gameObject.name = newName;
     }

     private Transform GetSpawnPointFree()
     {
          List<Transform> availableSpawnPoints = new();
          foreach (Transform spawnPoint in spawnPoints)
          {
               Collider[] hits =  Physics.OverlapSphere(spawnPoint.position, 10f, LayerMask.GetMask("Player"));
               if(hits.Length == 0) availableSpawnPoints.Add(spawnPoint);
          }

          if (availableSpawnPoints.Count > 0)
          {
               return availableSpawnPoints[UnityEngine.Random.Range(0, availableSpawnPoints.Count)];
          }
          else
          {
               return spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
          }
     }
}

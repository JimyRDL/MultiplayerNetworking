using System;
using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using UnityEngine;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine.UI;

public class PlayerLobbyControllerNB : NetworkBehaviour
{
     private NetworkManager networkManager;
     [SerializeField] private GameObject characterPrefab;
     [SerializeField] private List<GameObject> weapons = new();
     public readonly SyncVar<int> actualWeaponIndex = new SyncVar<int>();
     public readonly SyncVar<bool> IsReady = new SyncVar<bool>(false);
     public readonly SyncVar<GameManager.Teams> ActualTeamPlayer = new(GameManager.Teams.None);

     [Header("Canvas")] 
     [SerializeField] private GameObject canvasParent;
     [SerializeField] private GameObject lobbyCanvas;
     [SerializeField] private GameObject weaponSelectionCanvas;

     [Header("Lobby Items")] 
     [SerializeField] private Button goToWeaponSelectionButton;
     [SerializeField] private Button setReadyButton;
     [SerializeField] private Button startGameButton;
     [SerializeField] private Button blueTeamButton;
     [SerializeField] private Button redTeamButton;
     [SerializeField] private GameObject textPlayersNotReady;
     [SerializeField] private GameObject messageTheyCantBeReady;
     [SerializeField] private GameObject selectWeaponIndicator;
     
     [Header("Select Weapon Items")]
     [SerializeField] private List<Button> weaponButtons;
     [SerializeField] private Button weapon1Button;
     [SerializeField] private Transform selectionW1Transform;
     [SerializeField] private Button weapon2Button;
     [SerializeField] private Transform selectionW2Transform;
     
     
     private GameStarterManager gameStarterManager;

     private void Awake()
     {
          networkManager = InstanceFinder.NetworkManager;
          gameStarterManager = FindAnyObjectByType<GameStarterManager>();
     }

     public override void OnStartClient()
     {
          base.OnStartClient();
          canvasParent.SetActive(IsOwner);
          IsReady.OnChange += OnReadyChanged;
          ActualTeamPlayer.OnChange += OnTeamChanged;
          actualWeaponIndex.Value = -1;
          if (!IsServerStarted)
          {
               startGameButton.gameObject.SetActive(false);
          }
          if(IsOwner)
               Destroy(Camera.main.gameObject);
     }

     

     private void OnDestroy()
     {
          IsReady.OnChange -= OnReadyChanged;
          ActualTeamPlayer.OnChange -= OnTeamChanged;
     }


     private void Start()
     {
         // goToWeaponSelectionButton.onClick.AddListener(GoToWeaponSelection);
          setReadyButton.onClick.AddListener(SetReady);
          startGameButton.onClick.AddListener(StartGame);
          
          
          weapon1Button.onClick.AddListener(() => SelectWeapon(0, weapon1Button, selectionW1Transform));
          weapon2Button.onClick.AddListener(() => SelectWeapon(1, weapon2Button, selectionW2Transform));
          
          blueTeamButton.onClick.AddListener(() => SelectTeam(GameManager.Teams.Blue));
          redTeamButton.onClick.AddListener(() => SelectTeam(GameManager.Teams.Red));
     }

     private void OnTeamChanged(GameManager.Teams prev, GameManager.Teams next, bool asserver)
     {
          if (!IsOwner)
               return;
          if (next == GameManager.Teams.Blue)
          {
               blueTeamButton.image.color = Color.green;
               redTeamButton.image.color = Color.white;
          } else if (next == GameManager.Teams.Red)
          {
               blueTeamButton.image.color = Color.white;
               redTeamButton.image.color = Color.green;
          }
     }
     private void SelectTeam(GameManager.Teams team)
     {
          if (IsReady.Value)
               return;
          SelectTeamServerRPC(team);
         
     }

     [ServerRpc]
     private void SelectTeamServerRPC(GameManager.Teams team)
     {
          ActualTeamPlayer.Value = team;
     }

     private void StartGame()
     {
          if (!IsServerStarted)
               return;
          StartGameServer();
     }
     
     [Server]
     private void StartGameServer()
     {
          int connectionCount = networkManager.ServerManager.Clients.Count;
          int playersReady = 0;
          var players = new List<NetworkObject>(InstanceFinder.ServerManager.Objects.Spawned.Values);
          foreach (var obj in players)
          {
               if (obj.TryGetComponent(out PlayerLobbyControllerNB player))
               {
                    if(player.IsReady.Value)
                         playersReady++;
               }
          }

          if (connectionCount != playersReady)
          {
               textPlayersNotReady.SetActive(true);
               StartCoroutine(DeactivateText(textPlayersNotReady));
               return;
          }
          foreach (var obj in players)
          {
               if (obj.TryGetComponent<PlayerLobbyControllerNB>(out var player))
               {
                    player.SpawnCharacter(player.Owner, player.ActualTeamPlayer.Value);
               }
          }
          EventManager.Broadcast(Events.startCountdownEvent);
          ObserveStartGame();
     }

     private IEnumerator DeactivateText(GameObject text)
     {
          yield return new WaitForSeconds(3f);
          text.SetActive(false);
     }

     [ObserversRpc]
     private void ObserveStartGame()
     {
     }

     
     private void SetReady()
     {
          if (ActualTeamPlayer.Value == GameManager.Teams.None ||
              (actualWeaponIndex.Value != 0 && actualWeaponIndex.Value != 1))
          {
               messageTheyCantBeReady.SetActive(true);
               StartCoroutine(DeactivateText(messageTheyCantBeReady));
               return;
          }
          SetReadyServer();
     }
     private void OnReadyChanged(bool prev, bool next, bool asserver)
     {
          setReadyButton.image.color = next ? Color.green : Color.gray;
     }

     [ServerRpc]
     private void SetReadyServer()
     {
          IsReady.Value = !IsReady.Value;
     }
     

     private void GoToWeaponSelection()
     {
          
     }

     private void SelectWeapon(int index, Button actualButton, Transform selectionTransform)
     {
          if (IsReady.Value)
               return;
          foreach (var button in weaponButtons)
          {
               button.image.color = new Color32(255, 255, 255, 110);
          }
          selectWeaponIndicator.SetActive(true);
          selectWeaponIndicator.transform.position = selectionTransform.position;
          actualButton.image.color = Color.white;
          SelectActualWeaponServer(index);
     }

     [ServerRpc]
     private void SelectActualWeaponServer(int index)
     {
          actualWeaponIndex.Value = index;
     }
     

     [Server]
     private void SpawnCharacter(NetworkConnection conn, GameManager.Teams team)
     {
          GameObject player = Instantiate(characterPrefab);
          player.SetActive(false);
          ChangeNameObserverRPC(player, "Player " + conn);
          player.transform.position = gameStarterManager.GetNextSpawnPoint();
          player.SetActive(true);
          player.GetComponent<PlayerTeamManager>().team.Value = team;
          Spawn(player, conn);

          if (player.TryGetComponent(out PlayerWeaponManagerNB weaponManager))
          {
               weaponManager.SetupWeapon(weapons[actualWeaponIndex.Value], conn);
          }

      
          SpawnCharacterObserver();
     }

     [ObserversRpc]
     private void ChangeNameObserverRPC(GameObject player, string newName)
     {
          player.gameObject.name = newName;
     }
     
     [ObserversRpc]
     private void SpawnCharacterObserver()
     {
          Destroy(this.gameObject);
     }

     [ObserversRpc]
     private void DeactivateCanvasParent()
     {
          canvasParent.SetActive(false);
     }
}

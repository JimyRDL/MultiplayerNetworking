using System;
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

     [Header("Canvas")] 
     [SerializeField] private GameObject canvasParent;
     [SerializeField] private GameObject lobbyCanvas;
     [SerializeField] private GameObject weaponSelectionCanvas;

     [Header("Lobby Items")] 
     [SerializeField] private Button goToWeaponSelectionButton;
     [SerializeField] private Button setReadyButton;
     [SerializeField] private Button startGameButton;
     
     [Header("Select Weapon Items")]
     [SerializeField] private List<Button> weaponButtons;
     [SerializeField] private Button weapon1Button;
     [SerializeField] private Button weapon2Button;
     
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
          if (!IsServerStarted)
          {
               startGameButton.gameObject.SetActive(false);
          }
     }

     private void OnDestroy()
     {
          IsReady.OnChange -= OnReadyChanged;
     }


     private void Start()
     {
         // goToWeaponSelectionButton.onClick.AddListener(GoToWeaponSelection);
          setReadyButton.onClick.AddListener(SetReady);
          startGameButton.onClick.AddListener(StartGame);
          
          
          weapon1Button.onClick.AddListener(() => SelectWeapon(0, weapon1Button));
          weapon2Button.onClick.AddListener(() => SelectWeapon(1, weapon2Button));
          
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
               return;
          foreach (var obj in players)
          {
               if (obj.TryGetComponent<PlayerLobbyControllerNB>(out var player))
               {
                    player.SpawnCharacter(player.Owner);
               }
          }
          EventManager.Broadcast(Events.startCountdownEvent);
          ObserveStartGame();
     }

     [ObserversRpc]
     private void ObserveStartGame()
     {
     }

     
     private void SetReady()
     {
          SetReadyServer();
     }
     private void OnReadyChanged(bool prev, bool next, bool asserver)
     {
          setReadyButton.image.color = next ? Color.green : Color.red;
     }

     [ServerRpc]
     private void SetReadyServer()
     {
          IsReady.Value = !IsReady.Value;
     }
     

     private void GoToWeaponSelection()
     {
          
     }

     private void SelectWeapon(int index, Button actualButton)
     {
          foreach (var button in weaponButtons)
          {
               button.image.color = Color.gray;
          }

          actualButton.image.color = Color.green;
          SelectActualWeaponServer(index);
     }

     [ServerRpc]
     private void SelectActualWeaponServer(int index)
     {
          actualWeaponIndex.Value = index;
     }
     

     [Server]
     private void SpawnCharacter(NetworkConnection conn)
     {
          GameObject player = Instantiate(characterPrefab);
          player.SetActive(false);
          player.gameObject.name = "Player" + conn;
          player.transform.position = gameStarterManager.GetNextSpawnPoint();
          player.SetActive(true);
          Spawn(player, conn);

          if (player.TryGetComponent(out PlayerWeaponManagerNB weaponManager))
          {
               weaponManager.SetupWeapon(weapons[actualWeaponIndex.Value], conn);
          }

          SpawnCharacterObserver();
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

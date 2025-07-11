using System;
using FishNet;
using FishNet.Managing;
using FishNet.Transporting;
using FishNet.Transporting.Tugboat;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionLogicManager : MonoBehaviour
{
    
    private Tugboat tugboat;
    private NetworkManager networkManager;

    [Header("ConnectionLobby")]
    [SerializeField] private GameObject connectionLobbyCanvas;
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button joinClientButton;
    [SerializeField] private GameObject mainButtonsParent;
    [SerializeField] private GameObject enterHostAddressPanel;
    [SerializeField] private TMP_InputField enterHostAddressInput;
    [SerializeField] private Button tryJoinHostButton;
    [SerializeField] private Button closeHostAddressPanelButton;
    [SerializeField] private TextMeshProUGUI statusText;
    private bool hasJoined;
    

    private void OnEnable()
    {
        startHostButton.onClick.AddListener(StartHost);
        joinClientButton.onClick.AddListener(OpenJoinClient);
        tryJoinHostButton.onClick.AddListener(TryJoinHost);
        closeHostAddressPanelButton.onClick.AddListener(CloseHostAddressPanel);
        InstanceFinder.ClientManager.OnClientConnectionState += OnClientConnectionChanges;
    }
    private void OnDisable()
    {
        InstanceFinder.ClientManager.OnClientConnectionState -= OnClientConnectionChanges;
    }

    private void OnClientConnectionChanges(ClientConnectionStateArgs obj)
    {
        if (obj.ConnectionState == LocalConnectionState.Stopping && hasJoined)
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
#endif
        }

        if (obj.ConnectionState == LocalConnectionState.Started)
        {
            Debug.Log("Client Connection Started");
            Destroy(connectionLobbyCanvas);
            hasJoined = true;
        }

        if (obj.ConnectionState == LocalConnectionState.Stopped && !hasJoined)
        {
            Debug.LogWarning("Failed to connect to server");
            statusText.text = "No host was found with that address. Please enter a valid host address";
            statusText.color = Color.red;
        }
    }

    private void Awake()
    {
        networkManager = InstanceFinder.NetworkManager;
        
        if(networkManager.TryGetComponent(out Tugboat _t))
            tugboat = _t;
        else
            Debug.LogError("Tugboat not found");
    }


    private void CloseHostAddressPanel()
    {
        mainButtonsParent.SetActive(true);
        enterHostAddressPanel.SetActive(false);
    }

    private void TryJoinHost()
    {
        string hostAddress = enterHostAddressInput.text.Trim();
        if (string.IsNullOrEmpty(hostAddress))
        {
            statusText.text = "Please enter a valid host address";
            statusText.color = Color.red;
            return;
        }
        tugboat.SetClientAddress(hostAddress);
        networkManager.ClientManager.StartConnection();
        statusText.text = "Connecting to server...";
        statusText.color = Color.yellow;
    }

    private void OpenJoinClient()
    {
        #if UNITY_EDITOR
        tugboat.SetClientAddress("localhost");
        networkManager.ClientManager.StartConnection();
        #else
        mainButtonsParent.SetActive(false);
        enterHostAddressPanel.SetActive(true);
        #endif
    }

    private void StartHost()
    {
        tugboat.SetClientAddress("localhost");
        networkManager.ServerManager.StartConnection();
        networkManager.ClientManager.StartConnection();
        Destroy(connectionLobbyCanvas);
    }
    

    
    
}
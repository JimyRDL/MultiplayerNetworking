
using System;
using FishNet;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;

public class ConnectionManager: NetworkBehaviour
{
    private void OnEnable()
    {
        InstanceFinder.ClientManager.OnClientConnectionState += OnClientConnectionChanges;
    }
    private void OnDisable()
    {
        InstanceFinder.ClientManager.OnClientConnectionState -= OnClientConnectionChanges;
    }
    private void OnClientConnectionChanges(ClientConnectionStateArgs obj)
    {
        if (obj.ConnectionState == LocalConnectionState.Stopping)
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}




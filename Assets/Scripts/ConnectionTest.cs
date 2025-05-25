using System;
using FishNet;
using FishNet.Transporting;
using FishNet.Transporting.Tugboat;
using UnityEngine;

public class ConnectionTest : MonoBehaviour
{
    
    private Tugboat tugboat;
    

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
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }

    private void Start()
    {
        if (EditingManager.instance.Editing)
            return;
        if(TryGetComponent(out Tugboat _t))
        {
            tugboat = _t;
        } else
        {
            Debug.LogError("Tugboat not found");
            return;
        }


        if (ParrelSync.ClonesManager.IsClone())
        {
            tugboat.StartConnection(false);
        }
        else
        {
            tugboat.StartConnection(true);
            tugboat.StartConnection(false);
        }
    }
}
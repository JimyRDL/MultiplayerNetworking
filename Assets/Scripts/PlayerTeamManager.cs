
using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class PlayerTeamManager : NetworkBehaviour
{
    public readonly SyncVar<GameManager.Teams> team = new();

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!IsOwner)
            return;
        UIManager.Instance.SetLocalPlayer(this);
    }
}

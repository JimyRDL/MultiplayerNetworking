using System.Collections.Generic;
using FishNet.Connection;
using FishNet.Object;
using UnityEngine;
using UnityEngine.TextCore.Text;


public class CharacterSelection : NetworkBehaviour
{
    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private List<GameObject> weapons = new();
    [SerializeField] private GameObject canvasPlayer;


    public override void OnStartClient()
    {
        base.OnStartClient();
        enabled = IsOwner;
        canvasPlayer.SetActive(IsOwner);
    }

    public void SelectFirstPlayer()
    {
        SelectCharacterServer(0);
        canvasPlayer.SetActive(false);
    }

    public void SelectSecondPlayer()
    {
        SelectCharacterServer(1);
        canvasPlayer.SetActive(false);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SelectCharacterServer(int weaponIndex, NetworkConnection conn = null)
    {
        GameObject player = Instantiate(characterPrefab);
        player.SetActive(true);
        Spawn(player, conn);

        if (player.TryGetComponent(out PlayerWeaponManagerNB weaponManager))
        {
            weaponManager.SetupWeapon(weapons[weaponIndex], conn);
        }
    }
}

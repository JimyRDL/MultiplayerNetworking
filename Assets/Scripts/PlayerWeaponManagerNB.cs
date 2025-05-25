using FishNet.Connection;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponManagerNB : NetworkBehaviour
{
    
    public Transform weaponHolder;
    private Weapon actualWeapon;


    public override void OnStartClient()
    {
        base.OnStartClient();
        enabled = IsOwner;
    }
    
    public void SetupWeapon(GameObject weaponPrefab, NetworkConnection conn)
    {
        SpawnWeaponServer(weaponPrefab, conn);
    }
    
    private void SpawnWeaponServer(GameObject weaponPrefab, NetworkConnection conn)
    {
        GameObject weaponGO = Instantiate(weaponPrefab);
        Spawn(weaponGO, conn);
        actualWeapon = weaponGO.GetComponent<Weapon>();
    }
    
    
    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            Shoot();
    }

    private void Shoot()
    {
        throw new System.NotImplementedException();
    }
}

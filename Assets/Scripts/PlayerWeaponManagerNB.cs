using System;
using FishNet.Connection;
using FishNet.Example.ColliderRollbacks;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponManagerNB : NetworkBehaviour
{
    
    public Transform weaponHolder;
    private Weapon actualWeapon;

    private PlayerCameraControllerNB cameraController;
    [SerializeField] private LayerMask enemiesLayerMask;

    public override void OnStartClient()
    {
        base.OnStartClient();
        enabled = IsOwner;
    }

    private void Awake()
    {
        cameraController = GetComponent<PlayerCameraControllerNB>();
    }

    public void SetupWeapon(GameObject weaponPrefab, NetworkConnection conn)
    {
        SpawnWeaponServer(weaponPrefab, conn);
    }
    
    private void SpawnWeaponServer(GameObject weaponPrefab, NetworkConnection conn)
    {
        GameObject weaponGO = Instantiate(weaponPrefab);
        Spawn(weaponGO, conn);
        ObserverAssignWeapon(weaponGO.GetComponent<Weapon>());
    }

    [ObserversRpc]
    private void ObserverAssignWeapon(Weapon weapon)
    {
        actualWeapon = weapon;
    }
    
    
    public void OnShoot(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && actualWeapon != null)
            Shoot();
    }

    private void Shoot()
    {
        Debug.Log("Shoot");
        actualWeapon.Fire(transform,enemiesLayerMask);
    }
}

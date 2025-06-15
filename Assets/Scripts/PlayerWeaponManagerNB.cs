using System;
using FishNet.Connection;
using FishNet.Example.ColliderRollbacks;
using FishNet.Object;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponManagerNB : NetworkBehaviour
{
    [Header("Script References")]
    private Weapon actualWeapon;

    public GameObject ActualWeaponPrefab; 
    private PlayerTeamManager playerTeamManager;
    private PlayerCameraControllerNB cameraController;
    
    [Header("Scene References")]
    [SerializeField] private LayerMask enemiesLayerMask;
    public Transform weaponHolder;

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
        ObserverAssignWeapon(weaponGO.GetComponent<Weapon>(), weaponGO);
    }

    [ObserversRpc]
    private void ObserverAssignWeapon(Weapon weapon, GameObject weaponGO)
    {
        actualWeapon = weapon;
        ActualWeaponPrefab = weaponGO;
    }
    
    
    
    public void OnShoot(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && actualWeapon != null)
            Shoot();
    }

    private void Shoot()
    {
        actualWeapon.Fire(transform,enemiesLayerMask);
    }
}

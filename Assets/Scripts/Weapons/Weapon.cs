using System.Runtime.CompilerServices;
using FishNet.Object;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Weapon : NetworkBehaviour
{

    [SerializeField] protected int damage;
    [SerializeField] protected Transform firePosition;
    [SerializeField] protected GameObject bulletPrefab;
    protected PlayerWeaponManagerNB weaponManager;
    
    public override void OnStartClient()
    {
        base.OnStartClient();

        TryAttachToOwner();
        enabled = IsOwner;
    }

    private void TryAttachToOwner()
    {
        if (Owner == null) return;
        
        foreach (var obj in Owner.Objects)
        {
            weaponManager = obj.GetComponent<PlayerWeaponManagerNB>();
            if (weaponManager != null)
            {
                transform.SetParent(weaponManager.weaponHolder);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                break;
            }
        }
    }
    

    public virtual void Fire(Transform cameraTransform,LayerMask mask)
    {
        Vector3 origin = cameraTransform.position;
        Vector3 position = cameraTransform.forward;
        FireServer(origin, position, mask.value);
    }

    [ServerRpc(RequireOwnership = true)]
    private void FireServer(Vector3 origin , Vector3 direction, int mask)
    {
        Ray ray = new Ray(origin, direction);
        RaycastHit[] hits = Physics.RaycastAll(ray, 100f);

        RaycastHit? closestValidHit = null;
        float closestDistance = float.MaxValue;
        
        GameObject bullet = Instantiate(bulletPrefab, firePosition.position, Quaternion.identity);
        Spawn(bullet, Owner);
        bullet.GetComponent<BulletNB>().Setup(ray.direction.normalized, 350f);

        foreach (var hit in hits)
        {
            if (hit.collider.transform.root == transform.root)
                continue;
            if (hit.distance < closestDistance)
            {
                closestDistance = hit.distance;
                closestValidHit = hit;
            }
        }

        if (closestValidHit.HasValue)
        {
            var health = closestValidHit.Value.collider.transform.root.GetComponent<Health>();
            if (health != null)
            {
                weaponManager.ShowHitMarker(weaponManager.Owner);
                health.TakeDamage(damage);
            }
        }
    }
}

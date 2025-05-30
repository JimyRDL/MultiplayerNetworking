using System.Runtime.CompilerServices;
using FishNet.Object;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Weapon : NetworkBehaviour
{ 

    [SerializeField] protected int damage;
    
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
            var weaponManager = obj.GetComponent<PlayerWeaponManagerNB>();
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
        origin += new Vector3(0, 0.5f, 0);
        Ray ray = new Ray(origin, direction);
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
        RaycastHit[] hits = Physics.RaycastAll(ray, 100f);
        foreach (var hit in hits)
        {
            if (hit.collider.transform.root == transform.root)
                continue;
            Debug.Log(hit.collider.gameObject.name + "  hit");
            
            var health = hit.collider.transform.root.GetComponent<Health>();
            if (health != null)
            {
                Debug.Log(hit);
                health.TakeDamage(damage);
                break;
            }
            
        }
        Debug.Log("FiredServer");
    }
}

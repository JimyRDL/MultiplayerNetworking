using FishNet.Object;
using UnityEngine;

public class Weapon : NetworkBehaviour
{
    public override void OnStartClient()
    {
        base.OnStartClient();

        TryAttachToOwner();
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
}

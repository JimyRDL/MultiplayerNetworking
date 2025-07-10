using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
public class SniperWeaponSlow : Weapon
{
    private float cooldown = 2f;
    private bool inCooldown = false;
    public override void Fire(Transform cameraTransform,LayerMask mask)
    {
        if (!inCooldown)
        {
            base.Fire(cameraTransform, mask);
            inCooldown = true;
            StartCoroutine(StartCooldown());
        }
    }

    private IEnumerator StartCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        inCooldown = false;
    }
}


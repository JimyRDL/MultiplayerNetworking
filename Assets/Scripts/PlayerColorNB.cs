using UnityEngine;
using FishNet.Object;
using UnityEngine.InputSystem;

public class PlayerColorNB : NetworkBehaviour
{
     [SerializeField]
     private GameObject body;

     [SerializeField] private Color color;
     
     public override void OnStartClient()
     {
          base.OnStartClient();
          enabled = IsOwner;
     }


     public void OnChangeColor(InputAction.CallbackContext ctx)
     {
          if(ctx.performed)
               ChangeColorServer(color);
               
     }

     [ServerRpc(RequireOwnership = true)]
     public void ChangeColorServer(Color color)
     {
          ChangeColorObserver(color);
     }

     [ObserversRpc]
     private void ChangeColorObserver(Color color)
     {
          ChangeColorLocal(color);
     }

     private void ChangeColorLocal(Color color)
     {
          if (body.TryGetComponent(out Renderer renderer))
          {
               renderer.material.color = color;
          }
          else
          {
               Debug.LogError("No renderer found");
          }
     }
}

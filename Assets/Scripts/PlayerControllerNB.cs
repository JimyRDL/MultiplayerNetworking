using System;
using UnityEngine;
using FishNet.Object;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerNB : NetworkBehaviour
{
     [Header("Movement Variables")] 
     [SerializeField] private float movementSpeed = 10f;
     [SerializeField] private float jumpForce = 10f;
     [SerializeField] private float lookXSpeed;
     [SerializeField] private float lookYSpeed;
     [SerializeField] private float checkGroundRadius;

     private Vector2 moveInput;
     private Vector3 movement;
     private Vector2 lookInput;
     
     [Header("References")]
     private Camera playerCamera;
     private Rigidbody playerRigidbody;
     [SerializeField] private Transform cameraTransform;
     [SerializeField] private LayerMask groundLayer;
     [SerializeField] private Transform checkGroundTransform;
     [SerializeField] private MeshRenderer playerRenderer;

     [Header("States")]
     private bool canMove = true;
     private bool isGrounded = true;

     public override void OnStartClient()
     {
          base.OnStartClient();
          if (base.IsOwner)
          {
               playerCamera = Camera.main;
               playerCamera.transform.position =cameraTransform.position;
               playerCamera.transform.SetParent(transform);
               playerRenderer.enabled = false;
               
          } else 
               gameObject.GetComponent<PlayerControllerNB>().enabled = false; 
     }

     private void Awake()
     {
          playerRigidbody = GetComponent<Rigidbody>();
     }

     private void FixedUpdate()
     {
          MovePlayer();
          DetectGround();
     }
     

     private void MovePlayer()
     {
          if (moveInput == Vector2.zero || !canMove)
               return;
          movement = new Vector3(moveInput.x, 0, moveInput.y);
          playerRigidbody.MovePosition(playerRigidbody.position + movement * (movementSpeed * Time.fixedDeltaTime));
     }
     private void DetectGround()
     {
          isGrounded = Physics.CheckSphere(checkGroundTransform.position, checkGroundRadius, groundLayer);
     }

     private void OnDrawGizmos()
     {
          Gizmos.DrawWireSphere(checkGroundTransform.position, checkGroundRadius);
          Gizmos.color = Color.red;
     }

     public void OnMove(InputAction.CallbackContext ctx)
     {
          moveInput = ctx.ReadValue<Vector2>();
     }

     public void OnJump(InputAction.CallbackContext ctx)
     {
          if(ctx.performed && isGrounded)
               playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
     }

     public void OnLook(InputAction.CallbackContext ctx)
     {
          
     }
}

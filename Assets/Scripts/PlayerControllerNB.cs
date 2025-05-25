using System;
using System.Collections;
using UnityEngine;
using FishNet.Object;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerNB : NetworkBehaviour
{
     [Header("Movement Variables")] 
     [SerializeField] private float movementSpeed = 10f;
     [SerializeField] private float jumpForce = 10f;
     [SerializeField] private float checkGroundRadius;
     [SerializeField] private float gravityPower = 15f;

     private Vector2 moveInput;
     private Vector3 movement;

     
     [Header("References")]
     private Rigidbody playerRigidbody;
     [SerializeField] private LayerMask groundLayer;
     [SerializeField] private Transform checkGroundTransform;
     [SerializeField] private MeshRenderer playerRenderer;
     [SerializeField] private InputActionReference lookInputReference;

     [Header("States")]
     private bool canMove = true;
     private bool isGrounded = true;

     public override void OnStartClient()
     {
          base.OnStartClient();
          enabled = IsOwner;
          playerRenderer.enabled = !IsOwner;
     }

     private void Awake()
     {
          playerRigidbody = GetComponent<Rigidbody>();
     }

     private void Start()
     {
          Cursor.lockState = CursorLockMode.Locked;
     }
     

     private void FixedUpdate()
     {
          MovePlayer();
          DetectGround();
          ApplyGravity();
     }
     private void ApplyGravity()
     {
          if (isGrounded)
               playerRigidbody.AddForce(Vector3.down * gravityPower, ForceMode.Acceleration);
     }


     private void MovePlayer()
     {
          if (moveInput == Vector2.zero || !canMove)
               return;
          movement = transform.forward * moveInput.y + transform.right * moveInput.x;
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
          if (!IsOwner)
               return;
          moveInput = ctx.ReadValue<Vector2>();
     }

     public void OnJump(InputAction.CallbackContext ctx)
     {
          if (!IsOwner)
               return;
          if(ctx.performed && isGrounded)
               playerRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
     }
     
}

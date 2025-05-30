
using System;
using FishNet.Object;
using UnityEngine;

public class PlayerCameraControllerNB : NetworkBehaviour
{
    [SerializeField] private float lookHorizontalSpeed;
    [SerializeField] private float lookVerticalSpeed;
    
    private Vector2 lookInput;
    private float verticalRotation;
    private float horizontalRotation;
    
    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    private Camera playerCamera;
    public Camera PlayerCamera => playerCamera;

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner)
        {
            playerCamera = Camera.main;
            playerCamera.transform.SetParent(cameraTransform);
            playerCamera.transform.localPosition = Vector3.zero;
            playerCamera.transform.localRotation = Quaternion.identity;
            playerCamera.fieldOfView = 90;
            playerCamera.nearClipPlane = 0.01f;
        }
        else
        {
            gameObject.GetComponent<PlayerCameraControllerNB>().enabled = false; 
        }
    }

    private void Update()
    {
        LookCamera();
    }

    private void LookCamera()
    {
        float lookHorizontal = lookHorizontalSpeed  * Input.GetAxis("Mouse X") * Time.deltaTime;
        float lookVertical = lookVerticalSpeed * Input.GetAxis("Mouse Y") * Time.deltaTime;
          
        verticalRotation -= lookVertical;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
          
        horizontalRotation += lookHorizontal;
          
        transform.localRotation = Quaternion.Euler(0f, horizontalRotation, 0f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class CharacterInputSystem : MonoBehaviour
{
    private Rigidbody playerRb;
    private PlayerInputActions playerInputActions;
    private Transform cameraTransform;

    private float xRotation = 0;
    private float speed;

    private bool isSneaking = false;
    private float startingYScale;
    private float targetYScale;
    private float currentYScale;

    [Header("Crouching")]
    [SerializeField] private float crouchSpeed = 5;
    [SerializeField] private float crouchYScale = 0.6f;

    [Header("Movement speed")]
    [SerializeField] private float walkSpeed = 20;
    [SerializeField] private float sneakSpeed = 10;

    [Header("Sensivity")]
    [SerializeField] private float mouseSensivity = 15;

    private void Awake()
    {
        //assign proper values
        playerRb = GetComponent<Rigidbody>();
        playerRb.freezeRotation = true;
        speed = walkSpeed;

        playerInputActions = new PlayerInputActions();
        cameraTransform = GameObject.Find("MainCamera").transform;
        startingYScale = targetYScale = currentYScale = transform.localScale.y;
}

    private void FixedUpdate()
    {
        RotateCharacter();
        Sneaking();
        MoveCharacter();
    }

    private void RotateCharacter()
    {
        //move up or down
        float mouseY = playerInputActions.Player.MouseY.ReadValue<float>() * mouseSensivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0, 0);

        //move left or right
        float mouseX = playerInputActions.Player.MouseX.ReadValue<float>() * mouseSensivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);
    }

    private void Sneaking()
    {
        if (playerInputActions.Player.Sneak.ReadValue<float>() > 0 && !isSneaking)
        {
            //if Sneak input, apply slower speed and crouch
            speed = sneakSpeed;
            targetYScale = crouchYScale;
            isSneaking = true;
        }
        else if (playerInputActions.Player.Sneak.ReadValue<float>() == 0 && isSneaking)
        {
            //position of point from where ray will be casted
            CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();
            float capsuleRadius = capsuleCollider.radius * transform.localScale.x;
            Vector3 castOrigin1 = transform.position + new Vector3(0, (currentYScale * capsuleCollider.height) / 2, 0);
            Vector3 castOrigin2 = transform.position + new Vector3(capsuleRadius, (currentYScale * capsuleCollider.height) / 2, 0);
            Vector3 castOrigin3 = transform.position + new Vector3(-capsuleRadius, (currentYScale * capsuleCollider.height) / 2, 0);
            Vector3 castOrigin4 = transform.position + new Vector3(0, (currentYScale * capsuleCollider.height) / 2, capsuleRadius);
            Vector3 castOrigin5 = transform.position + new Vector3(0, (currentYScale * capsuleCollider.height) / 2, -capsuleRadius);
            //height dfifference between normal and rescaled player
            float heightDifference = capsuleCollider.height * (startingYScale - crouchYScale);
            //check if player is able to stand up
            if (!Physics.Raycast(castOrigin1, Vector3.up, heightDifference + 0.2f) &&
                !Physics.Raycast(castOrigin2, Vector3.up, heightDifference + 0.2f) &&
                !Physics.Raycast(castOrigin3, Vector3.up, heightDifference + 0.2f) &&
                !Physics.Raycast(castOrigin4, Vector3.up, heightDifference + 0.2f) &&
                !Physics.Raycast(castOrigin5, Vector3.up, heightDifference + 0.2f))
            {
                //if no Sneak input and able to stand up, apply faster speed and stand up
                speed = walkSpeed;
                targetYScale = startingYScale;
                isSneaking = false;
            }
        }

        if (!Mathf.Approximately(currentYScale, targetYScale)) 
        {
            currentYScale = Mathf.Lerp(currentYScale, targetYScale, crouchSpeed * Time.deltaTime);
            transform.localScale = new Vector3(transform.localScale.x, currentYScale, transform.localScale.z);
        }
    }

    private void MoveCharacter()
    {
        Vector2 inputVector = playerInputActions.Player.Movement.ReadValue<Vector2>();
        Vector3 movement = (transform.right * inputVector.x + transform.forward * inputVector.y) * speed;
        playerRb.AddForce(movement, ForceMode.Force);    
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        playerInputActions.Player.Enable();
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Confined;
        playerInputActions.Player.Disable();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class OldCharacterInputSystem : MonoBehaviour
{
    private Rigidbody playerRb;
    private CapsuleCollider capsuleCollider;
    private Transform cameraTransform;
    private PlayerInputActions playerInputActions;

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
        capsuleCollider = GetComponent<CapsuleCollider>();
        cameraTransform = GameObject.Find("MainCamera").transform;
        playerInputActions = new PlayerInputActions();

        playerRb.freezeRotation = true;
        speed = walkSpeed;
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
        if (playerInputActions.Player.Sneaking.ReadValue<float>() > 0 && !isSneaking)
        {
            //if Sneak input, apply slower speed and crouch
            speed = sneakSpeed;
            targetYScale = crouchYScale;
            isSneaking = true;
        }
        else if (playerInputActions.Player.Sneaking.ReadValue<float>() == 0 && isSneaking)
        {
            //position of point from where ray will be casted
            float capsuleRadius = capsuleCollider.radius * transform.localScale.x;
            float castDistance = capsuleCollider.height * (startingYScale - crouchYScale / 2);
            castDistance = castDistance - capsuleRadius - 0.01f;
            if (!Physics.SphereCast(transform.position, capsuleRadius, Vector3.up, out RaycastHit hitInfo, castDistance))
            {

                //if no Sneak input and able to stand up, apply faster speed and stand up
                speed = walkSpeed;
                targetYScale = startingYScale;
                isSneaking = false;
            }
            else
            {
                Debug.Log(hitInfo.transform.gameObject.name);
                Debug.Log(transform.position);
                Debug.Log(castDistance);
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

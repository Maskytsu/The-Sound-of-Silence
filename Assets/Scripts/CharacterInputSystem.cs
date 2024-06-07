using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputSystem : MonoBehaviour
{
    private Transform cameraTransform;
    private PlayerInputActions playerInputActions;

    private float xRotation = 0;
    private float speed;

    private bool isSneaking = false;

    [Header("Crouching")]
    [SerializeField] private float crouchSpeed = 5;

    [Header("Movement speed")]
    [SerializeField] private float walkSpeed = 20;
    [SerializeField] private float sneakSpeed = 10;

    [Header("Sensivity")]
    [SerializeField] private float mouseSensivity = 15;

    private void Awake()
    {
        //assign proper values
        cameraTransform = GameObject.Find("MainCamera").transform;
        playerInputActions = new PlayerInputActions();

        speed = walkSpeed;
    }

    private void FixedUpdate()
    {
        RotateCharacter();
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
        Debug.Log(mouseY + " " + mouseX);
    }
}

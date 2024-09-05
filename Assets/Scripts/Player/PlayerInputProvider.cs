using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputProvider : MonoBehaviour
{
    public PlayerInputActions PlayerInputActions { get; private set; }

    private void Awake()
    {
        PlayerInputActions = new PlayerInputActions();
        Cursor.lockState = CursorLockMode.Locked;
        PlayerInputActions.PlayerMap.Enable();
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        PlayerInputActions.PlayerMap.Enable();
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Confined;
        PlayerInputActions.PlayerMap.Disable();
    }
}

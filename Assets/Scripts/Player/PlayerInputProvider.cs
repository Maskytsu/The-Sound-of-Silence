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

    public void TurnOnPlayerMap()
    {
        PlayerInputActions.PlayerMap.Enable();
    }

    public void TurnOffPlayerMap()
    {
        PlayerInputActions.PlayerMap.Disable();
    }

    public void TurnOnPlayerMapLockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        PlayerInputActions.PlayerMap.Enable();
    }

    public void TurnOffPlayerMapUnlockCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
        PlayerInputActions.PlayerMap.Disable();
    }
}

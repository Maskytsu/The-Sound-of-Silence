using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputProvider : MonoBehaviour
{
    public static InputProvider Instance { get; private set; }

    public PlayerInputActions PlayerInputActions { get; private set; }
    public PlayerInputActions.PlayerMapActions PlayerMap { get; private set; }
    public PlayerInputActions.UIMouseMapActions UIMouseMap { get; private set; }

    private void Awake()
    {
        CreateInstance();

        PlayerInputActions = new PlayerInputActions();
        Cursor.lockState = CursorLockMode.Locked;

        PlayerMap = PlayerInputActions.PlayerMap;
        UIMouseMap = PlayerInputActions.UIMouseMap;

        PlayerInputActions.PlayerMap.Enable();
        PlayerInputActions.UIMouseMap.Enable();
    }

    private void OnDisable()
    {
        PlayerInputActions.PlayerMap.Disable();
        PlayerInputActions.UIMouseMap.Disable();
    }

    private void CreateInstance()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one InputProvider in the scene.");
        }
        Instance = this;
    }

    public void TurnOnPlayerMap()
    {
        PlayerInputActions.PlayerMap.Enable();
    }

    public void TurnOffPlayerMap()
    {
        PlayerInputActions.PlayerMap.Disable();
    }

    public void TurnOnUIMouseMap()
    {
        PlayerInputActions.UIMouseMap.Enable();
    }

    public void TurnOffUIMouseMap()
    {
        PlayerInputActions.UIMouseMap.Disable();
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }
}

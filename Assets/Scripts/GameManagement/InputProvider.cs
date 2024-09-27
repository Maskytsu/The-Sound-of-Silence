using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputProvider : MonoBehaviour
{
    public static InputProvider Instance { get; private set; }

    public PlayerInputActions PlayerInputActions { get; private set; }
    public PlayerInputActions.PlayerKeyboardMapActions PlayerKeyboardMap { get; private set; }
    public PlayerInputActions.PlayerMouseMapActions PlayerMouseMap { get; private set; }
    public PlayerInputActions.UIMouseMapActions UIMouseMap { get; private set; }

    private void Awake()
    {
        CreateInstance();

        PlayerInputActions = new PlayerInputActions();
        Cursor.lockState = CursorLockMode.Locked;

        PlayerKeyboardMap = PlayerInputActions.PlayerKeyboardMap;
        PlayerMouseMap = PlayerInputActions.PlayerMouseMap;
        UIMouseMap = PlayerInputActions.UIMouseMap;

        PlayerInputActions.PlayerKeyboardMap.Enable();
        PlayerInputActions.PlayerMouseMap.Enable();
        PlayerInputActions.UIMouseMap.Enable();
    }

    private void OnDisable()
    {
        PlayerInputActions.PlayerKeyboardMap.Disable();
        PlayerInputActions.PlayerMouseMap.Disable();
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

    public void TurnOnPlayerMaps()
    {
        PlayerInputActions.PlayerKeyboardMap.Enable();
        PlayerInputActions.PlayerMouseMap.Enable();
    }

    public void TurnOffPlayerMaps()
    {
        PlayerInputActions.PlayerKeyboardMap.Disable();
        PlayerInputActions.PlayerMouseMap.Disable();
    }

    public void TurnOnPlayerKeyboardMap()
    {
        PlayerInputActions.PlayerKeyboardMap.Enable();
    }

    public void TurnOffPlayerKeyboardMap()
    {
        PlayerInputActions.PlayerKeyboardMap.Disable();
    }

    public void TurnOnPlayerMouseMap()
    {
        PlayerInputActions.PlayerMouseMap.Enable();
    }

    public void TurnOffPlayerMouseMap()
    {
        PlayerInputActions.PlayerMouseMap.Disable();
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
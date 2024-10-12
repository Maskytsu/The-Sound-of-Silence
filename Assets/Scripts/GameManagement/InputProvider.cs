using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputProvider : MonoBehaviour
{
    public static InputProvider Instance { get; private set; }

    public PlayerInputActions PlayerInputActions { get; private set; }
    public PlayerInputActions.PlayerKeyboardMapActions PlayerKeyboardMap { get; private set; }
    public PlayerInputActions.PlayerMouseMapActions PlayerMouseMap { get; private set; }
    public PlayerInputActions.UICustomMapActions UICustomMap { get; private set; }

    [SerializeField] private GameManager _gameManager;

    private void Awake()
    {
        CreateInstance();

        PlayerInputActions = new PlayerInputActions();

        PlayerKeyboardMap = PlayerInputActions.PlayerKeyboardMap;
        PlayerMouseMap = PlayerInputActions.PlayerMouseMap;
        UICustomMap = PlayerInputActions.UICustomMap;
        
        if (_gameManager.IsGameplayScene)
        {
            Cursor.lockState = CursorLockMode.Locked;

            PlayerKeyboardMap.Enable();
            PlayerMouseMap.Enable();
            UICustomMap.Enable();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;

            PlayerMouseMap.Enable();
        }
    }

    private void OnDisable()
    {
        PlayerKeyboardMap.Disable();
        PlayerMouseMap.Disable();
        UICustomMap.Disable();
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
        PlayerKeyboardMap.Enable();
        PlayerMouseMap.Enable();
    }

    public void TurnOffPlayerMaps()
    {
        PlayerKeyboardMap.Disable();
        PlayerMouseMap.Disable();
    }

    public void TurnOnPlayerKeyboardMap()
    {
        PlayerKeyboardMap.Enable();
    }

    public void TurnOffPlayerKeyboardMap()
    {
        PlayerKeyboardMap.Disable();
    }

    public void TurnOnPlayerMouseMap()
    {
        PlayerMouseMap.Enable();
    }

    public void TurnOffPlayerMouseMap()
    {
        PlayerMouseMap.Disable();
    }

    public void TurnOnUIMouseMap()
    {
        UICustomMap.Enable();
    }

    public void TurnOffUIMouseMap()
    {
        UICustomMap.Disable();
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
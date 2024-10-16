using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputProvider : MonoBehaviour
{
    public static InputProvider Instance { get; private set; }

    public PlayerInputActions PlayerInputActions { get; private set; }
    public PlayerInputActions.PlayerMovementMapActions PlayerMovementMap { get; private set; }
    public PlayerInputActions.PlayerMainMapActions PlayerMainMap { get; private set; }
    public PlayerInputActions.UICustomMapActions UICustomMap { get; private set; }

    [SerializeField] private GameManager _gameManager;

    private void Awake()
    {
        CreateInstance();
        SetupInput();
    }

    private void OnDisable()
    {
        PlayerMovementMap.Disable();
        PlayerMainMap.Disable();
        UICustomMap.Disable();
    }

    public void TurnOnPlayerMaps()
    {
        PlayerMovementMap.Enable();
        PlayerMainMap.Enable();
    }

    public void TurnOffPlayerMaps()
    {
        PlayerMovementMap.Disable();
        PlayerMainMap.Disable();
    }

    public void TurnOnPlayerMovementMap()
    {
        PlayerMovementMap.Enable();
    }

    public void TurnOffPlayerMovementMap()
    {
        PlayerMovementMap.Disable();
    }

    public void TurnOnPlayerMainMap()
    {
        PlayerMainMap.Enable();
    }

    public void TurnOffPlayerMainMap()
    {
        PlayerMainMap.Disable();
    }

    public void TurnOnUICustomMap()
    {
        UICustomMap.Enable();
    }

    public void TurnOffUICustomMap()
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

    private void SetupInput()
    {
        PlayerInputActions = new PlayerInputActions();

        PlayerMovementMap = PlayerInputActions.PlayerMovementMap;
        PlayerMainMap = PlayerInputActions.PlayerMainMap;
        UICustomMap = PlayerInputActions.UICustomMap;

        if (_gameManager.IsGameplayScene)
        {
            Cursor.lockState = CursorLockMode.Locked;

            PlayerMovementMap.Enable();
            PlayerMainMap.Enable();
            UICustomMap.Enable();
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;

            UICustomMap.Enable();
        }
    }

    private void CreateInstance()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one InputProvider in the scene.");
        }
        Instance = this;
    }
}
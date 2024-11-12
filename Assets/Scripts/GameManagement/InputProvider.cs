using NaughtyAttributes;
using UnityEngine;

public class InputProvider : MonoBehaviour
{
    public static InputProvider Instance { get; private set; }

    public PlayerInputActions PlayerInputActions { get; private set; }
    public PlayerInputActions.PlayerMovementMapActions PlayerMovementMap { get; private set; }
    public PlayerInputActions.PlayerCameraMapActions PlayerCameraMap { get; private set; }
    public PlayerInputActions.GameplayOverlayMapActions GameplayOverlayMap { get; private set; }
    public PlayerInputActions.UIMapActions UIMap { get; private set; }

    [SerializeField] private SceneSetup _sceneSetup;
    [Space]
    [SerializeField, Tooltip("Only for testing scenes!")] private bool _activateAllMaps;

    [ShowNativeProperty] private bool _playerMovementMapEnabled => Application.isPlaying && PlayerMovementMap.enabled;
    [ShowNativeProperty] private bool _playerCameraMapEnabled => Application.isPlaying && PlayerCameraMap.enabled;
    [ShowNativeProperty] private bool _gameplayOverlayMapEnabled => Application.isPlaying && GameplayOverlayMap.enabled;

    private InputMapStates _savedInputMapStates;

    private void Awake()
    {
        CreateInstance();
        SetupInput();
    }

    public void SaveMapStates()
    {
        _savedInputMapStates = new InputMapStates(
            _playerMovementMapEnabled, 
            _playerCameraMapEnabled, 
            _gameplayOverlayMapEnabled);
    }

    public void LoadMapStatesAndApplyThem()
    {
        if (_savedInputMapStates == null)
        {
            Debug.LogError("No map states saved!");
            return;
        }

        if (_savedInputMapStates.PlayerCameraMapEnabled) PlayerCameraMap.Enable();
        else PlayerCameraMap.Disable();

        if (_savedInputMapStates.PlayerMovementMapEnabled) PlayerMovementMap.Enable();
        else PlayerMovementMap.Disable();

        if (_savedInputMapStates.GameplayOverlayMapEnabled) GameplayOverlayMap.Enable();
        else GameplayOverlayMap.Disable();

        _savedInputMapStates = null;
    }

    //PlayerMaps
    public void TurnOnPlayerMaps()
    {
        PlayerMovementMap.Enable();
        PlayerCameraMap.Enable();
    }
    public void TurnOffPlayerMaps()
    {
        PlayerMovementMap.Disable();
        PlayerCameraMap.Disable();
    }

    //GameplayMaps
    public void TurnOnGameplayMaps()
    {
        PlayerMovementMap.Enable();
        PlayerCameraMap.Enable();
        GameplayOverlayMap.Enable();
    }
    public void TurnOffGameplayMaps()
    {
        PlayerMovementMap.Disable();
        PlayerCameraMap.Disable();
        GameplayOverlayMap.Disable();
    }

    //PlayerMovementMap
    public void TurnOnPlayerMovementMap()
    {
        PlayerMovementMap.Enable();
    }
    public void TurnOffPlayerMovementMap()
    {
        PlayerMovementMap.Disable();
    }

    //PlayerCameraMap
    public void TurnOnPlayerCameraMap()
    {
        PlayerCameraMap.Enable();
    }
    public void TurnOffPlayerCameraMap()
    {
        PlayerCameraMap.Disable();
    }

    //GameplayOverlayMap
    public void TurnOnGameplayOverlayMap()
    {
        GameplayOverlayMap.Enable();
    }
    public void TurnOffGameplayOverlayMap()
    {
        GameplayOverlayMap.Disable();
    }

    //Cursor
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    //Setup
    private void SetupInput()
    {
        PlayerInputActions = new PlayerInputActions();

        PlayerMovementMap = PlayerInputActions.PlayerMovementMap;
        PlayerCameraMap = PlayerInputActions.PlayerCameraMap;
        GameplayOverlayMap = PlayerInputActions.GameplayOverlayMap;
        UIMap = PlayerInputActions.UIMap;

        if (_activateAllMaps)
        {
            PlayerMovementMap.Enable();
            PlayerCameraMap.Enable();
            GameplayOverlayMap.Enable();
        }

        UIMap.Enable();

        if (_sceneSetup.LockCursor) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.Confined;
    }

    private void CreateInstance()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one InputProvider in the scene.");
        }
        Instance = this;
    }

    private class InputMapStates
    {
        public bool PlayerMovementMapEnabled;
        public bool PlayerCameraMapEnabled;
        public bool GameplayOverlayMapEnabled;

        public InputMapStates(bool PlayerMovementMapState, bool PlayerCameraMapState, bool GameplayOverlayMapState)
        {
            PlayerMovementMapEnabled = PlayerMovementMapState;
            PlayerCameraMapEnabled  = PlayerCameraMapState;
            GameplayOverlayMapEnabled = GameplayOverlayMapState;
        }
    }
}
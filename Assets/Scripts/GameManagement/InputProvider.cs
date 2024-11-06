using NaughtyAttributes;
using UnityEngine;

public class InputProvider : MonoBehaviour
{
    public static InputProvider Instance { get; private set; }

    public PlayerInputActions PlayerInputActions { get; private set; }
    public PlayerInputActions.PlayerMovementMapActions PlayerMovementMap { get; private set; }
    public PlayerInputActions.PlayerMainMapActions PlayerMainMap { get; private set; }
    public PlayerInputActions.UICustomMapActions UICustomMap { get; private set; }

    [SerializeField] private SceneSetup _sceneSetup;

    [ShowNativeProperty] private bool _playerMovementMapEnabled => Application.isPlaying && PlayerMovementMap.enabled;
    [ShowNativeProperty] private bool _playerMainMapEnabled => Application.isPlaying && PlayerMainMap.enabled;
    [ShowNativeProperty] private bool _uiCustomMapEnabled => Application.isPlaying && UICustomMap.enabled;

    private void Awake()
    {
        CreateInstance();
        SetupInput();
    }

    private void OnDestroy()
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

        if (_sceneSetup.ActivatePlayerMovementMap) PlayerMovementMap.Enable();
        if (_sceneSetup.ActivatePlayerMainMap) PlayerMainMap.Enable();
        if (_sceneSetup.ActivateUIMap) UICustomMap.Enable();
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
}
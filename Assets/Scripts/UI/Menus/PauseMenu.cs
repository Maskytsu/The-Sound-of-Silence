using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _menu;
    [SerializeField] private GameObject _settings;
    [SerializeField] private GameObject _controls;
    [SerializeField] private GameObject _mainMenuWizard;
    [SerializeField] private GameObject _quitGameWizard;
    [Space]
    [SerializeField, Scene] private string _mainMenuScene;

    private InputProvider _inputProvider;

    private bool _savedPlayerMovementMapEnabled;
    private bool _savedPlayerMainMapEnabled;

    private void Awake()
    {
        Time.timeScale = 0f;

        SetupInput();
    }

    private void Update()
    {
        ManageKeyboardInput();
    }

    public void CloseMenu()
    {
        Time.timeScale = 1f;

        _inputProvider.LoadMapStatesAndApplyThem();
        _inputProvider.LockCursor();

        Destroy(gameObject);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(_mainMenuScene);
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    private void SetupInput()
    {
        _inputProvider = InputProvider.Instance;

        _inputProvider.SaveMapStates();
        _inputProvider.TurnOffGameplayMaps();
        _inputProvider.UnlockCursor();
    }

    private void ManageKeyboardInput()
    {
        if (_inputProvider.UIMap.Cancel.WasPerformedThisFrame())
        {
            if (_menu.activeSelf)
            {
                CloseMenu();
            }
            else
            {
                _menu.SetActive(true);

                _settings.SetActive(false);
                _controls.SetActive(false);
                _mainMenuWizard.SetActive(false);
                _quitGameWizard.SetActive(false);
            }
        }
    }
}

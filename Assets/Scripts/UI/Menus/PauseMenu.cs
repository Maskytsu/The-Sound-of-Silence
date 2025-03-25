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

    private InputProvider _inputProvider => InputProvider.Instance;

    private void Awake()
    {
        Time.timeScale = 0f;
        AudioManager.Instance.PauseGameplaySounds(true, true);

        _inputProvider.SaveMapStates();
        _inputProvider.TurnOffGameplayMaps();
        _inputProvider.UnlockCursor();
    }

    private void Update()
    {
        ManageKeyboardInput();
    }

    public void CloseMenu()
    {
        Time.timeScale = 1f;
        AudioManager.Instance.UnpauseGameplaySounds(true, true);

        _inputProvider.LoadMapStatesAndApplyThem();
        _inputProvider.LockCursor();

        Destroy(gameObject);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        AudioManager.Instance.StopGamplaySoundsAndUnpauseThem();

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

using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _menu;
    [SerializeField] private GameObject _newGameWizard;
    [SerializeField] private GameObject _settings;
    [SerializeField] private GameObject _quitGameWizard;
    [Space]
    [SerializeField] private GameObject _continueButton;
    [Space]
    [SerializeField] private GameObject _newGameButtonFast;
    [SerializeField] private GameObject _newGameButtonWizard;
    [Space]
    [SerializeField, Scene] private string _firstGameplayScene;

    private PlayerInputActions.UIMapActions UIMap => InputProvider.Instance.UIMap;

    private void Awake()
    {
        SetupMainButtons();
    }

    private void Update()
    {
        ManageKeyboardInput();
    }

    public void StartNewGame()
    {
        SaveManager.Instance.ClearSave();

        SceneManager.LoadScene(_firstGameplayScene);
    }

    public void ContinueSavedScene()
    {
        SaveManager.Instance.LoadSavedScene();
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    private void SetupMainButtons()
    {
        if (PlayerPrefs.GetString("SavedScene") == "")
        {
            _continueButton.SetActive(false);

            _newGameButtonFast.SetActive(true);
            _newGameButtonWizard.SetActive(false);
        }
        else
        {
            _continueButton.SetActive(true);

            _newGameButtonFast.SetActive(false);
            _newGameButtonWizard.SetActive(true);
        }
    }

    private void ManageKeyboardInput()
    {
        if (UIMap.Cancel.WasPerformedThisFrame() && !_menu.activeSelf)
        {
            _menu.SetActive(true);

            _newGameWizard.SetActive(false);
            _settings.SetActive(false);
            _quitGameWizard.SetActive(false);
        }
    }
}
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

    private PlayerInputActions.UICustomMapActions UICustomMap => InputProvider.Instance.UICustomMap;

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
        PlayerPrefs.SetInt("CheckedMechanic", 0);
        PlayerPrefs.SetInt("MessageSentToMechanic", 0);
        PlayerPrefs.SetInt("MessageSentToClaire", 0);
        PlayerPrefs.SetInt("CalledToClaire", 0);
        PlayerPrefs.SetInt("CheckedPolice", 0);
        PlayerPrefs.SetInt("CalledToPolice", 0);
        PlayerPrefs.SetInt("TookPills", 0);

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
        if (UICustomMap.Cancel.WasPerformedThisFrame() && !_menu.activeSelf)
        {
            _menu.SetActive(true);

            _newGameWizard.SetActive(false);
            _settings.SetActive(false);
            _quitGameWizard.SetActive(false);
        }
    }
}
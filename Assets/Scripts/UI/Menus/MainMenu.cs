using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private CanvasGroup _menuGroup;
    [Space]
    [SerializeField] private Button _continueButton;
    [SerializeField] private GameObject _continueGreyButton;
    [Space]
    [SerializeField] private Button _newGameButtonFast;
    [SerializeField] private Button _newGameButtonWizard;
    [Space]
    [SerializeField, Scene] private string _firstGameplayScene;

    private float _fadeDuration = 0.2f;

    private PlayerInputActions.UIMapActions UIMap => InputProvider.Instance.UIMap;
    private CanvasGroup _currentGroup;

    private void Awake()
    {
        SetupMainButtons();
        _currentGroup = _menuGroup;
    }

    private void Update()
    {
        ManageKeyboardInput();
    }

    public void SetCurrentGroup(CanvasGroup group)
    {
        _currentGroup.interactable = false;
        _currentGroup.blocksRaycasts = false;

        _currentGroup.DOFade(0.0f, _fadeDuration).onComplete += () => {
            _currentGroup = group;
            _currentGroup.DOFade(1.0f, _fadeDuration).onComplete += () =>
            {
                _currentGroup.interactable = true;
                _currentGroup.blocksRaycasts = true;
            };
        };
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
            _continueButton.gameObject.SetActive(false);
            _continueGreyButton.SetActive(true);

            _newGameButtonFast.gameObject.SetActive(true);
            _newGameButtonWizard.gameObject.SetActive(false);
        }
        else
        {
            _continueButton.gameObject.SetActive(true);
            _continueGreyButton.SetActive(false);

            _newGameButtonFast.gameObject.SetActive(false);
            _newGameButtonWizard.gameObject.SetActive(true);
        }
    }

    private void ManageKeyboardInput()
    {
        if (UIMap.Cancel.WasPerformedThisFrame() && _currentGroup != _menuGroup)
        {
            SetCurrentGroup(_menuGroup);
        }
    }
}
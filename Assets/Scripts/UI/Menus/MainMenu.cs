using DG.Tweening;
using FMODUnity;
using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private CanvasGroup _menuGroup;
    [SerializeField] private BlinkEffect _blink;
    [Space]
    [SerializeField] private Button _continueButton;
    [SerializeField] private GameObject _continueGreyButton;
    [Space]
    [SerializeField] private Button _endingHintsButton;
    [Space]
    [SerializeField] private Button _newGameButtonFast;
    [SerializeField] private Button _newGameButtonWizard;
    [Space]
    [SerializeField, Scene] private string _firstGameplayScene;
    [Space]

    private float _fadeDuration = 0.2f;
    private CanvasGroup _currentGroup;

    public bool IsKeyboardBackLocked = false;

    private PlayerInputActions.UIMapActions UIMap => InputProvider.Instance.UIMap;
    public float FadeDuration => _fadeDuration;

    private void Awake()
    {
        SetupMainButtons();
        _currentGroup = _menuGroup;
    }

    private IEnumerator Start()
    {
        SetupEndingHintsButton();
        UIMap.Disable();
        _blink.SetActiveFullBlackout(true);
        var openEyesSpeed = 3.0f;
        _blink.PlayOpenEyes(openEyesSpeed, true);
        yield return new WaitForSecondsRealtime(_blink.GetOpenEyesDuration(openEyesSpeed) + 0.2f);
        UIMap.Enable();
    }

    private void Update()
    {
        ManageKeyboardInput();
    }

    public void SetCurrentGroup(CanvasGroup group)
    {
        StartCoroutine(SetCurrentGroupRoutine(group));
    }

    public IEnumerator SetCurrentGroupRoutine(CanvasGroup group)
    {
        yield return StartCoroutine(FadeCurrentCanvasGroup(false));
        _currentGroup = group;
        yield return StartCoroutine(FadeCurrentCanvasGroup(true));
    }

    public void StartNewGame()
    {
        SaveManager.Instance.ClearSave();
        StartCoroutine(DelayActionAfterBlink(() => SceneManager.LoadScene(_firstGameplayScene)));
    }

    public void ContinueSavedScene()
    {
        StartCoroutine(DelayActionAfterBlink(() => SaveManager.Instance.LoadSavedScene()));
    }

    public void QuitGame()
    {
        StartCoroutine(DelayActionAfterBlink(() =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Application.Quit();
        }));
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

    private void SetupEndingHintsButton()
    {
        var showEndingHints = EndingsSaveManager.AnyEndingReached && !EndingsSaveManager.AllEndingsReached;
        _endingHintsButton.gameObject.SetActive(showEndingHints);
    }

    private void ManageKeyboardInput()
    {
        if (!IsKeyboardBackLocked && UIMap.Cancel.WasPerformedThisFrame() && _currentGroup != _menuGroup)
        {
            RuntimeManager.PlayOneShot(FmodEvents.Instance.UIDecline);
            SetCurrentGroup(_menuGroup);
        }
    }

    private IEnumerator DelayActionAfterBlink(Action action)
    {
        UIMap.Disable();
        var closeEyesSpeed = 3.0f;
        _blink.PlayCloseEyes(closeEyesSpeed, true);
        yield return new WaitForSecondsRealtime(_blink.GetCloseEyesDuration(closeEyesSpeed) + 0.2f);
        UIMap.Enable();

        action();
    }

    private IEnumerator FadeCurrentCanvasGroup(bool toShown)
    {
        if (!toShown)
        {
            _currentGroup.interactable = false;
            _currentGroup.blocksRaycasts = false;
        }

        Tween tween = _currentGroup.DOFade(toShown ? 1.0f : 0.0f, _fadeDuration).SetUpdate(true);
        while (tween.IsPlaying()) yield return null;

        if (toShown)
        {
            _currentGroup.interactable = true;
            _currentGroup.blocksRaycasts = true;
        }
    }
}
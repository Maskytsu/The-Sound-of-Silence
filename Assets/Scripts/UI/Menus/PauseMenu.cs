using DG.Tweening;
using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _menuObject;
    [SerializeField] private CanvasGroup _menuGroup;
    [SerializeField] private BlinkEffect _blink;
    [SerializeField, Scene] private string _mainMenuScene;

    private InputProvider InputProvider => InputProvider.Instance;

    private float _fadeDuration = 0.2f;
    private CanvasGroup _currentGroup;

    private IEnumerator Start()
    {
        TimeManager.Instance.PauseTimeScale();
        AudioManager.Instance.PauseGameplaySounds(true, true);

        InputProvider.SaveMapStates();
        InputProvider.TurnOffGameplayMaps();

        _currentGroup = _menuGroup;
        _menuObject.SetActive(false);

        InputProvider.UIMap.Disable();

        _blink.PlayCloseEyes(5.0f, true);
        while (_blink.IsPlaying) yield return null;
        yield return new WaitForSecondsRealtime(0.1f);
        _menuObject.SetActive(true);
        HUD.Instance.gameObject.SetActive(false);
        _blink.PlayOpenEyes(5.0f, true, false);
        while (_blink.IsPlaying) yield return null;

        InputProvider.UIMap.Enable();
        InputProvider.UnlockCursor();
    }

    private void Update()
    {
        ManageKeyboardInput();
    }

    public void SetCurrentGroup(CanvasGroup group)
    {
        _currentGroup.interactable = false;
        _currentGroup.blocksRaycasts = false;

        _currentGroup.DOFade(0.0f, _fadeDuration).SetUpdate(true).onComplete += () => {
            _currentGroup = group;
            _currentGroup.DOFade(1.0f, _fadeDuration).SetUpdate(true).onComplete += () =>
            {
                _currentGroup.interactable = true;
                _currentGroup.blocksRaycasts = true;
            };
        };
    }

    public void CloseMenu()
    {
        InputProvider.LockCursor();

        DOTween.KillAll();
        StartCoroutine(CloseMenuAnimation());
    }

    public void GoToMainMenu()
    {
        StartCoroutine(DelayActionAfterBlink(() =>
        {
            TimeManager.Instance.ResetTimeScale();
            AudioManager.Instance.StopGamplaySoundsAndUnpauseThem();

            SceneManager.LoadScene(_mainMenuScene);
        }));
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

    private void ManageKeyboardInput()
    {
        if (InputProvider.UIMap.Cancel.WasPerformedThisFrame())
        {
            if (_currentGroup != _menuGroup) SetCurrentGroup(_menuGroup);
            else CloseMenu();
        }
    }

    private IEnumerator DelayActionAfterBlink(Action action)
    {
        InputProvider.UIMap.Disable();
        var closeEyesSpeed = 3.0f;
        _blink.PlayCloseEyes(closeEyesSpeed, true);
        yield return new WaitForSecondsRealtime(_blink.GetCloseEyesDuration(closeEyesSpeed) + 0.2f);
        InputProvider.UIMap.Enable();

        action();
    }

    private IEnumerator CloseMenuAnimation()
    {
        InputProvider.UIMap.Disable();

        _blink.PlayCloseEyes(5.0f, true);
        while (_blink.IsPlaying) yield return null;
        yield return new WaitForSecondsRealtime(0.1f);
        _menuObject.SetActive(false);
        HUD.Instance.gameObject.SetActive(true);
        _blink.PlayOpenEyes(5.0f, true);
        while (_blink.IsPlaying) yield return null;

        InputProvider.UIMap.Enable();

        TimeManager.Instance.ResetTimeScale();
        AudioManager.Instance.UnpauseGameplaySounds(true, true);

        InputProvider.LoadMapStatesAndApplyThem();

        Destroy(gameObject);
    }
}
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Transform _mainTransform;
    [SerializeField] private CanvasGroup _menuGroup;
    [SerializeField, Scene] private string _mainMenuScene;

    private InputProvider InputProvider => InputProvider.Instance;

    private float _fadeDuration = 0.2f;
    private CanvasGroup _currentGroup;

    private Vector3 _cachedPosition;

    private void Awake()
    {
        TimeManager.Instance.PauseTimeScale();
        AudioManager.Instance.PauseGameplaySounds(true, true);

        InputProvider.SaveMapStates();
        InputProvider.TurnOffGameplayMaps();

        _currentGroup = _menuGroup;
        _cachedPosition = _mainTransform.localPosition;
        _mainTransform.DOLocalMove(Vector3.zero, 0.1f).SetUpdate(true).onComplete += () =>
        {
            InputProvider.UnlockCursor();
        };
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

        _mainTransform.DOLocalMove(_cachedPosition, 0.1f).SetUpdate(true).onComplete += () => {
            Destroy(gameObject);

            TimeManager.Instance.ResetTimeScale();
            AudioManager.Instance.UnpauseGameplaySounds(true, true);

            InputProvider.LoadMapStatesAndApplyThem();
        };
    }

    public void GoToMainMenu()
    {
        TimeManager.Instance.ResetTimeScale();
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
        if (InputProvider.UIMap.Cancel.WasPerformedThisFrame())
        {
            if (_currentGroup != _menuGroup) SetCurrentGroup(_menuGroup);
            else CloseMenu();
        }
    }
}

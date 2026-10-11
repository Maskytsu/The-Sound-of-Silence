using DG.Tweening;
using FMOD.Studio;
using UnityEngine;

public class TutorialOverlay : MonoBehaviour
{
    [SerializeField] private CanvasGroup _group;
    [SerializeField] private bool _updateIndependent;

    private float _fadeDuration = 0.4f;
    private Tween _fadeTween;
    private bool _isGettingDestroyed;

    protected virtual void Start()
    {
        _group.alpha = 0.0f;
        _fadeTween = _group.DOFade(1.0f, _fadeDuration).SetDelay(_fadeDuration + 0.1f).SetUpdate(_updateIndependent);
    }

    public void EndTutorial()
    {
        if (_fadeTween.IsActive())
        {
            _fadeTween.Kill();
        }

        _isGettingDestroyed = true;
        _fadeTween = _group.DOFade(0.0f, _fadeDuration).SetUpdate(_updateIndependent);
        _fadeTween.onComplete += () => Destroy(gameObject);
    }

    public void SetVisible(bool isVisible)
    {
        _fadeTween.Kill();

        if (_isGettingDestroyed) {
            Destroy(gameObject);
            return;
        }

        _group.alpha = isVisible ? 1f : 0f;
    }
}
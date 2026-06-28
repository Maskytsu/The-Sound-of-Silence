using DG.Tweening;
using UnityEngine;

public class TutorialOverlay : MonoBehaviour
{
    [SerializeField] private CanvasGroup _group;

    private float _fadeDuration = 0.4f;
    private Tween _fadeTween;

    protected virtual void Start()
    {
        _group.alpha = 0.0f;
        _fadeTween = _group.DOFade(1.0f, _fadeDuration).SetDelay(_fadeDuration + 0.1f);
    }

    public void EndTutorial()
    {
        if (_fadeTween.IsActive())
        {
            _fadeTween.Kill();
        }

        _fadeTween = _group.DOFade(0.0f, _fadeDuration);
        _fadeTween.onComplete += () => Destroy(gameObject);
    }
}
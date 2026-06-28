using DG.Tweening;
using TMPro;
using UnityEngine;

public class QuestText : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI TMP;
    [SerializeField] protected CanvasGroup _group;

    protected float _fadeDuration = 0.4f;
    protected Tween _fadeTween;

    protected virtual void Start()
    {
        _group.alpha = 0.0f;
        _fadeTween = _group.DOFade(1.0f, _fadeDuration).SetDelay(_fadeDuration + 0.1f);
    }

    public virtual void DestroyQuestText()
    {
        if (_fadeTween.IsActive())
        {
            _fadeTween.Kill();
        }

        _fadeTween = _group.DOFade(0.0f, _fadeDuration);
        _fadeTween.onComplete += () => Destroy(gameObject);
    }
}
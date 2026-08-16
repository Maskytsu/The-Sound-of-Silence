using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ExhaustEffect : MonoBehaviour
{
    [SerializeField] private Image _exhaustOverlay;
    [SerializeField] private Volume _exhaustVolume;

    public void StartEffect(float duration)
    {
        DOTween.Kill(this);
        VolumeTween(1.0f, duration);
        _exhaustOverlay.DOFade(1.0f, duration);
    }

    public void EndEffect(float duration)
    {
        DOTween.Kill(this);
        VolumeTween(0.0f, duration);
        _exhaustOverlay.DOFade(0.0f, duration);
    }

    public void InstantEndEffect()
    {
        DOTween.Kill(this);
        _exhaustVolume.weight = 0.0f;
        _exhaustOverlay.color = new Color(_exhaustOverlay.color.r, _exhaustOverlay.color.g, _exhaustOverlay.color.b, 0.0f);
    }

    private Tween VolumeTween(float targetWeight, float duration) => DOTween.To(() => _exhaustVolume.weight, x => _exhaustVolume.weight = x, targetWeight, duration);
}

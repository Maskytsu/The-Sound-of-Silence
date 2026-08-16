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
        VolumeTween(1.0f, duration);
        _exhaustOverlay.DOFade(1.0f, duration);
    }

    public void EndEffect(float duration)
    {
        VolumeTween(0.0f, duration);
        _exhaustOverlay.DOFade(0.0f, duration);
    }

    private Tween VolumeTween(float targetWeight, float duration) => DOTween.To(() => _exhaustVolume.weight, x => _exhaustVolume.weight = x, targetWeight, duration);
}

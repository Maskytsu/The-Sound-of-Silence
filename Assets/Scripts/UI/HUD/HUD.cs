using DG.Tweening;
using UnityEngine;

public class HUD : SingletonMonobehaviour<HUD>
{
    public QuestDisplay QuestDisplay;
    public GameObject MiddlePointer;
    public Blackout Blackout;
    public BlinkEffect Blink;
    public DialogueDisplay DialogueDisplay;

    [SerializeField] private CanvasGroup _mainCanvasGroup;

    public Tween FadeAnimaton(float duration, bool toVisible, bool isIndependentUpdate = false)
    {
        return _mainCanvasGroup.DOFade(toVisible ? 1.0f : 0.0f, duration).SetUpdate(isIndependentUpdate);
    }

    public void SetVisible(bool isVisible)
    {
        _mainCanvasGroup.alpha = isVisible ? 1f : 0f;
    }
}
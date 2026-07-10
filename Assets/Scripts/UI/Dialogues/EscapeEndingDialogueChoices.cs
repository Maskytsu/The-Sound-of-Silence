using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EscapeEndingDialogueChoices : MonoBehaviour
{
    public event Action<EndingChoice> OnChoiceMade;
    [HideInInspector] public EscapeEndingQuestHandler QuestHandler;

    [SerializeField] private CanvasGroup _mainCanvasGroup;
    [SerializeField] private Button _optionChris;
    [SerializeField] private Button _optionClaire;
    [SerializeField] private Button _optionPolice;

    private float _fadingSpeed = 0.6f;

    private void Start()
    {
        _mainCanvasGroup.alpha = 0.0f;
        _mainCanvasGroup.interactable = false;

        _optionChris.onClick.AddListener(() => Choose(EndingChoice.Chris));
        _optionClaire.onClick.AddListener(() => Choose(EndingChoice.Claire));
        _optionPolice.onClick.AddListener(() => Choose(EndingChoice.Police));

        StartCoroutine(DisplayButtons());
    }

    private void Choose(EndingChoice choice)
    {
        InputProvider.Instance.LockCursor();
        _mainCanvasGroup.interactable = false;
        OnChoiceMade?.Invoke(choice);
    }

    private IEnumerator DisplayButtons()
    {
        HUD.Instance.FadeAnimaton(_fadingSpeed / 2.0f, false);
        Tween fadingInTMPTween = _mainCanvasGroup.DOFade(1f, _fadingSpeed);
        while (fadingInTMPTween.IsActive()) yield return null;

        InputProvider.Instance.UnlockCursor();
        _mainCanvasGroup.interactable = true;
    }

    public IEnumerator HideButtons()
    {
        HUD.Instance.FadeAnimaton(_fadingSpeed / 2.0f, true);
        Tween fadingInTMPTween = _mainCanvasGroup.DOFade(0f, _fadingSpeed);
        while (fadingInTMPTween.IsActive()) yield return null;
    }
}
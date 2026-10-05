using DG.Tweening;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndingHintsMenu : MonoBehaviour
{
    [SerializeField] private EndingHintButton _hintPrefab;
    [SerializeField] private Transform _hintsParent;
    [SerializeField] private Button _backButton;
    [Space]
    [SerializeField] private MainMenu _mainMenu;
    [SerializeField] private CanvasGroup _mainCanvasGroup;
    [SerializeField] private CanvasGroup _hintCanvasGroup;
    [SerializeField] private TextMeshProUGUI _hintTMP;
    [Space]
    [SerializeField] private List<HintSetup> _hintSetups = new();

    private bool _isHintShown = false;

    private void Start()
    {
        foreach(var hintSetup in _hintSetups)
        {
            if (!EndingsSaveManager.EndingStates[hintSetup.endingNumber]())
            {
                var spawnedHint = Instantiate(_hintPrefab, _hintsParent);
                spawnedHint.Setup(this, hintSetup);
            }
        }

        _backButton.transform.SetAsLastSibling();
    }

    private void Update()
    {
        if (_isHintShown && InputProvider.Instance.UIMap.Cancel.WasPerformedThisFrame())
        {
            HideHint();
        }
    }

    public IEnumerator ShowHintRoutine(string hintText)
    {
        _mainMenu.IsKeyboardBackLocked = true;
        _hintTMP.text = hintText;
        yield return StartCoroutine(FadeCanvasGroup(_mainCanvasGroup, false));
        yield return StartCoroutine(FadeCanvasGroup(_hintCanvasGroup, true));
        _isHintShown = true;
    }

    public void HideHint()
    {
        StartCoroutine(HideHintRoutine());
    }

    private IEnumerator HideHintRoutine()
    {
        _isHintShown = false;
        yield return StartCoroutine(FadeCanvasGroup(_hintCanvasGroup, false));
        yield return StartCoroutine(FadeCanvasGroup(_mainCanvasGroup, true));
        _mainMenu.IsKeyboardBackLocked = false;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, bool toShown)
    {
        if (!toShown)
        {
            group.interactable = false;
            group.blocksRaycasts = false;
        }

        Tween tween = group.DOFade(toShown ? 1.0f : 0.0f, _mainMenu.FadeDuration).SetUpdate(true);
        while (tween.IsPlaying()) yield return null;

        if (toShown)
        {
            group.interactable = true;
            group.blocksRaycasts = true;
        }
    }
}


[Serializable]
public class HintSetup
{
    public int endingNumber;
    public string buttonText;
    [TextArea]
    public string hintText;
}
using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueDisplay : MonoBehaviour
{
    [SerializeField] private CanvasGroup _group;
    [SerializeField] private Image _background;
    [SerializeField] private TextMeshProUGUI _dialogueTMP;
    [SerializeField] private DialogueColorSetup _colorSetup;

    private float _fadeDuration = 0.4f;
    private float _resizeDuration = 0.25f;

    private DialogueSequenceScriptable _currentActiveSequence;
    private Vector2 AdjustedBackgroundSize => new(_dialogueTMP.textBounds.size.x + 100.0f, _dialogueTMP.textBounds.size.y + 100.0f);

    public void DisplayDialogue(DialogueSequenceScriptable dialogueSequence, float delay = 0.0f)
    {
        if (_currentActiveSequence != null)
        {
            Debug.LogError("Tried to display dialogue while other was active, it was closed and new one started!");
            StopAllCoroutines();
            StartCoroutine(CancelCurrentDialogueAndStartNew(dialogueSequence, delay));
            return;
        }

        _currentActiveSequence = dialogueSequence;
        StartCoroutine(DisplayDialogueCoroutine(dialogueSequence, delay));
    }

    public IEnumerator DisplayDialogueCoroutine(DialogueSequenceScriptable dialogueSequence, float delay)
    {
        if (delay > 0.0f)
        {
            yield return new WaitForSeconds(delay);
        }

        var firstDialogueLine = _currentActiveSequence.DialogueLines[0];
        SetDialogueLine(firstDialogueLine);
        yield return new WaitForSeconds(0.0f); //delay to apply text
        _background.rectTransform.sizeDelta = AdjustedBackgroundSize;

        yield return _group.DOFade(1.0f, _fadeDuration).WaitForCompletion();
        yield return new WaitForSeconds(firstDialogueLine.DisplayTime);

        for (int i = 1; i < _currentActiveSequence.DialogueLines.Count; i++)
        {
            yield return _dialogueTMP.DOFade(0.0f, _fadeDuration).WaitForCompletion();

            var dialogueLine = _currentActiveSequence.DialogueLines[i];
            SetDialogueLine(dialogueLine);
            SetDialogueTextAlpha(0.0f);
            yield return new WaitForSeconds(0.0f); //delay to apply text

            yield return _background.rectTransform.DOSizeDelta(AdjustedBackgroundSize, _resizeDuration).WaitForCompletion();
            yield return _dialogueTMP.DOFade(1.0f, _fadeDuration).WaitForCompletion();
            yield return new WaitForSeconds(dialogueLine.DisplayTime);
        }

        yield return _group.DOFade(0.0f, _fadeDuration).WaitForCompletion();

        _currentActiveSequence.EndDialogue();
        _currentActiveSequence = null;
    }

    private IEnumerator CancelCurrentDialogueAndStartNew(DialogueSequenceScriptable dialogueSequence, float delay)
    {
        DOTween.Kill(this);
        _currentActiveSequence.EndDialogue();
        yield return _group.DOFade(0.0f, 0.2f).WaitForCompletion();

        _currentActiveSequence = dialogueSequence;
        yield return StartCoroutine(DisplayDialogueCoroutine(dialogueSequence, delay));
    }

    private void SetDialogueLine(DialogueSequenceScriptable.DialogueLine dialogueLine)
    {
        _dialogueTMP.text = dialogueLine.Text;
        _dialogueTMP.color = _colorSetup.GetColor(dialogueLine.ColorType);
    }

    private void SetDialogueTextAlpha(float alpha)
    {
        _dialogueTMP.color = new (_dialogueTMP.color.r, _dialogueTMP.color.g, _dialogueTMP.color.b, alpha);
    }
}
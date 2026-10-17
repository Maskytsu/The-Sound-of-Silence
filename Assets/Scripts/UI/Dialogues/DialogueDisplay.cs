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

    private float _fadeDuration = 0.4f;
    private float _resizeDuration = 0.25f;
    private bool _displayHiddenByEvent = false;

    private DialogueSequenceScriptable _currentActiveSequence;
    private Vector2 AdjustedBackgroundSize => new(_dialogueTMP.textBounds.size.x + 100.0f, _dialogueTMP.textBounds.size.y + 100.0f);

    public void DisplayDialogue(DialogueSequenceScriptable dialogueSequence, float delay = 0.0f)
    {
        if (_currentActiveSequence != null)
        {
            Debug.LogError("Tried to display dialogue while other was active, it was closed and new one started!");
            _currentActiveSequence.EndDialogue();
            StopAllCoroutines();
            StartCoroutine(CancelCurrentDialogueCoroutine(newDialogueSequence: dialogueSequence, delay));
            return;
        }

        _currentActiveSequence = dialogueSequence;
        StartCoroutine(DisplayDialogueCoroutine(dialogueSequence, delay));
    }

    public void CancelCurrentDialogue()
    {
        if (_currentActiveSequence != null)
        {
            _currentActiveSequence.EndDialogue();
            StopAllCoroutines();
            StartCoroutine(CancelCurrentDialogueCoroutine());
        }
    }

    public IEnumerator DisplayDialogueCoroutine(DialogueSequenceScriptable dialogueSequence, float delay)
    {
        if (delay > 0.0f)
        {
            yield return new WaitForSeconds(delay);
        }

        var currentDialogueLine = _currentActiveSequence.DialogueLines[0];
        SetDialogueLine(dialogueSequence, currentDialogueLine);
        yield return new WaitForSeconds(0.0f); //delay to apply text
        _background.rectTransform.sizeDelta = AdjustedBackgroundSize;

        yield return _group.DOFade(1.0f, _fadeDuration).WaitForCompletion();
        yield return new WaitForSeconds(currentDialogueLine.Duration);

        for (int i = 1; i < _currentActiveSequence.DialogueLines.Count; i++)
        {
            currentDialogueLine = _currentActiveSequence.DialogueLines[i];

            if (currentDialogueLine.IsEvent)
            {
                yield return StartCoroutine(HandleDialogueEvent(currentDialogueLine));
                continue;
            }

            if (_displayHiddenByEvent)
            {
                SetDialogueLine(dialogueSequence, currentDialogueLine);
                _displayHiddenByEvent = false;
                _dialogueTMP.alpha = 1.0f;
                yield return new WaitForSeconds(0.0f); //delay to apply text
                _background.rectTransform.sizeDelta = AdjustedBackgroundSize;
                yield return _group.DOFade(1.0f, _fadeDuration).WaitForCompletion();
            }
            else
            {
                yield return _dialogueTMP.DOFade(0.0f, _fadeDuration).WaitForCompletion();
                SetDialogueLine(dialogueSequence, currentDialogueLine);
                SetDialogueTextAlpha(0.0f);
                yield return new WaitForSeconds(0.0f); //delay to apply text
                yield return _background.rectTransform.DOSizeDelta(AdjustedBackgroundSize, _resizeDuration).WaitForCompletion();
                yield return _dialogueTMP.DOFade(1.0f, _fadeDuration).WaitForCompletion();
            }

            yield return new WaitForSeconds(currentDialogueLine.Duration);
        }

        yield return _group.DOFade(0.0f, _fadeDuration).WaitForCompletion();

        _currentActiveSequence.EndDialogue();
        _currentActiveSequence = null;
    }

    private IEnumerator CancelCurrentDialogueCoroutine(DialogueSequenceScriptable newDialogueSequence = null, float delay = 0f)
    {
        DOTween.Kill(this);
        _currentActiveSequence.EndDialogue();
        yield return _group.DOFade(0.0f, 0.2f).WaitForCompletion();

        _currentActiveSequence = newDialogueSequence;
        if (newDialogueSequence != null)
        {
            yield return StartCoroutine(DisplayDialogueCoroutine(newDialogueSequence, delay));
        }
    }

    private void SetDialogueLine(DialogueSequenceScriptable dialogueSequence, DialogueSequenceScriptable.DialogueLine dialogueLine)
    {
        string text = dialogueLine.Text;

        if (dialogueSequence.IsThoughts)
        {
            text = "(" + text.Replace("\n", ")\n(") + ")";
        }

        _dialogueTMP.text = text;
        _dialogueTMP.color = UIColors.Instance.GetDialogueColor(dialogueLine.ColorType);
    }

    private void SetDialogueTextAlpha(float alpha)
    {
        _dialogueTMP.color = new (_dialogueTMP.color.r, _dialogueTMP.color.g, _dialogueTMP.color.b, alpha);
    }

    private IEnumerator HandleDialogueEvent(DialogueSequenceScriptable.DialogueLine dialogueLine)
    {
        if (dialogueLine.Event.HideDialogueDisplay)
        {
            _displayHiddenByEvent = true;
            yield return _group.DOFade(0.0f, _fadeDuration).WaitForCompletion();
        }

        dialogueLine.HandleEvent();
        yield return new WaitForSeconds(dialogueLine.Duration);
    }
}
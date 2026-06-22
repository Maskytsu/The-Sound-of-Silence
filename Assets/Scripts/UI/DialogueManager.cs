using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueManager : SingletonMonobehaviour<DialogueManager>
{
    private DialogueSequenceScriptable _currentActiveSequence;

    private DialogueDisplay Display => HUD.Instance.DialogueDisplay;

    public void DisplayDialogue(DialogueSequenceScriptable dialogueSequence, float delay = 0.0f)
    {
        if (_currentActiveSequence != null)
        {
            Debug.LogError("Tried to display dialogue while other was active, ld was closed and new one started!");
            StopAllCoroutines();
            _currentActiveSequence.EndDialogue();
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

        Display.SetActiveTextBox(true);

        foreach (var dialogueLine in _currentActiveSequence.DialogueLines)
        {
            Display.SetCurrentLine(dialogueLine);
            yield return new WaitForSeconds(dialogueLine.DisplayTime);
        }

        _currentActiveSequence.EndDialogue();
        _currentActiveSequence = null;

        Display.SetActiveTextBox(false);
    }
}
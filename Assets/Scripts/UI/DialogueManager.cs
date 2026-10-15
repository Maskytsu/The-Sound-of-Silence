using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager : SingletonMonobehaviour<DialogueManager>
{
    private DialogueDisplay Display => HUD.Instance.DialogueDisplay;
    public void DisplayDialogue(DialogueSequenceScriptable dialogueSequence, float delay = 0.0f) => Display.DisplayDialogue(dialogueSequence, delay);
    public void CancelCurrentDialogue() => Display.CancelCurrentDialogue();
}
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager : SingletonMonobehaviour<DialogueManager>
{
    private DialogueDisplay Display => HUD.Instance.DialogueDisplay;
    public static void DisplayDialogue(DialogueSequenceScriptable dialogueSequence, float delay = 0.0f) => Instance.Display.DisplayDialogue(dialogueSequence, delay);
    public static void CancelCurrentDialogue() => Instance.Display.CancelCurrentDialogue();
}
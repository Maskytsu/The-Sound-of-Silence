using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueManager : SingletonMonobehaviour<DialogueManager>
{
    [SerializeField] private DialogueSequenceScriptable _testingSequence;
    
    private DialogueDisplay Display => HUD.Instance.DialogueDisplay;

    private void Start()
    {
        InputProvider.Instance.DebugMap.PlayTestDialogue.performed += PlayTestDialogue;
    }

    private void OnDestroy()
    {
        InputProvider.Instance.DebugMap.PlayTestDialogue.performed -= PlayTestDialogue;
    }

    public void DisplayDialogue(DialogueSequenceScriptable dialogueSequence, float delay = 0.0f)
    {
        Display.DisplayDialogue(dialogueSequence, delay);
    }

    private void PlayTestDialogue(InputAction.CallbackContext context)
    {
        DisplayDialogue(_testingSequence);
    }
}
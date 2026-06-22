using UnityEngine;

public class DialogueInteracion : Interactable
{
    [SerializeField] protected DialogueSequenceScriptable _dialogueSequence;
    [SerializeField] protected float delay = 0.5f;

    protected override void Interact()
    {
        DialogueManager.Instance.DisplayDialogue(_dialogueSequence, delay);
        _interactionHitbox.gameObject.SetActive(false);
    }
}
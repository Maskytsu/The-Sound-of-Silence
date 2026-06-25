using UnityEngine;

public class DialogueInteracion : Interactable
{
    public InteractionHitbox InteractionHitbox => _interactionHitbox;

    [SerializeField] protected DialogueSequenceScriptable _dialogueSequence;
    [SerializeField] protected float delay = 0.5f;

    protected override string GizmoIconName => "GreenInteractionIcon.png";

    protected override void Interact()
    {
        DialogueManager.Instance.DisplayDialogue(_dialogueSequence, delay);
        _interactionHitbox.gameObject.SetActive(false);
    }
}
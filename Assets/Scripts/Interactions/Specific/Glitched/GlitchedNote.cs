using System;
using UnityEngine;

public abstract class GlitchedNote : Note
{
    [SerializeField] private bool _isGlitched;
    [SerializeField] private GameObject _glitchOverlay;
    [Space]
    [SerializeField] private DialogueSequenceScriptable _dialogueSequence;
    [SerializeField] private float _dialogueDelay = 0.5f;

    protected override void Awake()
    {
        base.Awake();
        _glitchOverlay.SetActive(_isGlitched);
    }

    protected abstract void SetGameStateValue();

    protected override void Interact()
    {
        if (!_isGlitched)
        {
            SetGameStateValue();
            base.Interact();
            return;
        }

        DialogueManager.Instance.DisplayDialogue(_dialogueSequence, _dialogueDelay);
        _interactionHitbox.gameObject.SetActive(false);
    }
}
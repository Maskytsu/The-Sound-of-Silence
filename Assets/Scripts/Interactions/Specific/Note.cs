using NaughtyAttributes;
using System;
using UnityEngine;

public class Note : Interactable
{
    public Action OnFirstReadingEnd;

    public InteractionHitbox InteractionHitbox => _interactionHitbox;

    [Space]
    [SerializeField] private PaperSheetDisplay _noteDisplay;

    private bool _noteWasRead = false;

    protected override void Interact()
    {
        PaperSheetDisplay _spawnedNoteDisplay = Instantiate(_noteDisplay);
        if (!_noteWasRead) _spawnedNoteDisplay.OnReadingEnd += InvokeOnFirstReadingEndEvent;
    }

    private void InvokeOnFirstReadingEndEvent()
    {
        _noteDisplay.OnReadingEnd -= InvokeOnFirstReadingEndEvent;
        _noteWasRead = true;
        OnFirstReadingEnd?.Invoke();
    }
}
using UnityEngine;

public abstract class DialogueEvent : ScriptableObject
{
    public bool HideDialogueDisplay;

    public abstract float EventDuration { get; }
    public abstract void HandleEvent();
}
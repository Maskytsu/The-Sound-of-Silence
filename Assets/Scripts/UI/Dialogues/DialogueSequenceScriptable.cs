using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSequence", menuName = "ScriptableObjects/DialogueSequence")]
public class DialogueSequenceScriptable : ScriptableObject
{
#if UNITY_EDITOR
    public static bool EditingEvents;
#endif

    public bool IsThoughts;
    public List<DialogueLine> DialogueLines;
    public event Action OnDialogueEnd;

    public float GetDialogueDuration()
    {
        float timeOfDialogues = 0;

        foreach (var dialogueLine in DialogueLines)
        {
            timeOfDialogues += dialogueLine.Duration;
        }

        return timeOfDialogues;
    }

    public void EndDialogue()
    {
        OnDialogueEnd?.Invoke();
        OnDialogueEnd = null;
    }

    [Button]
    private void PlayThisDialogue()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.DisplayDialogue(this);
        }
        else
        {
            Debug.LogWarning("Dialogue Manager Instance doesn't exist!");
        }
    }

    //--------------------------------------------------------------------------------

    [Serializable]
    public class DialogueLine
    {
        public bool IsEvent;
        public DialogueEvent Event;

        public DialogueColorType ColorType;
        [TextArea(2, 4)] public string Text;

        public float Duration => IsEvent ? Event.EventDuration : Text.GetReadingTime(true);
        public void HandleEvent() => Event.HandleEvent();
    }
}
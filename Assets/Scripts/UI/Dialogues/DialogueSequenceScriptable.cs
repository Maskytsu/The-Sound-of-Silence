using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSequence", menuName = "ScriptableObjects/DialogueSequence")]
public class DialogueSequenceScriptable : ScriptableObject
{
    public bool IsThoughts;
    public List<DialogueLine> DialogueLines;
    public event Action OnDialogueEnd;

    [Serializable]
    public class DialogueLine
    {
        public DialogueColorType ColorType;
        public float DisplayTime = 4f; //obsolete
        [TextArea(2, 4)]
        public string Text;

        public float ReadingTime => Text.GetReadingTime() / Settings.Instance.DialogueSpeed.Value;
    }

    public float GetDialogueDuration()
    {
        float timeOfDialogues = 0;

        foreach(var dialogueLine in DialogueLines)
        {
            timeOfDialogues += dialogueLine.ReadingTime;
        }

        return timeOfDialogues;
    }

    public void EndDialogue()
    {
        OnDialogueEnd?.Invoke();
        OnDialogueEnd = null;
    }
}
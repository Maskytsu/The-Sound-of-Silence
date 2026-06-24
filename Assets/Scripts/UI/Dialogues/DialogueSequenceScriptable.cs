using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSequence", menuName = "ScriptableObjects/DialogueSequence")]
public class DialogueSequenceScriptable : ScriptableObject
{
    public List<DialogueLine> DialogueLines;
    public event Action OnDialogueEnd;

    [Serializable]
    public class DialogueLine
    {
        public DialogueColorType ColorType;
        public float DisplayTime = 4f;
        [TextArea(2, 4)]
        public string Text;
    }

    public float GetDialogueDuration()
    {
        float timeOfDialogues = 0;

        foreach(var dialogueLine in DialogueLines)
        {
            timeOfDialogues += dialogueLine.DisplayTime;
        }

        return timeOfDialogues;
    }

    public void EndDialogue()
    {
        OnDialogueEnd?.Invoke();
        OnDialogueEnd = null;
    }
}
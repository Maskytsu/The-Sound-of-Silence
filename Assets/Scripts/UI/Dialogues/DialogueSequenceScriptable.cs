using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSequence", menuName = "ScriptableObjects/DialogueSequence")]
public class DialogueSequenceScriptable : ScriptableObject
{
    public List<DialogueLine> DialogueLines;
    public Action OnDialogueEnd;

    [Serializable]
    public class DialogueLine
    {
        public Color TextColor;
        public float DisplayTime = 4f;
        [TextArea(2, 4)]
        public string Text;
    }

    public void EndDialogue()
    {
        Debug.Log("Dialogue ended.");
        OnDialogueEnd?.Invoke();
    }
}
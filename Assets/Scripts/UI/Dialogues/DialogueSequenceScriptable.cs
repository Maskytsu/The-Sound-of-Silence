using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSequence", menuName = "ScriptableObjects/DialogueSequence")]
public class DialogueSequenceScriptable : ScriptableObject
{
    public List<DialogueLine> DialogueLines;

    [Serializable]
    public class DialogueLine
    {
        public Color TextColor;
        public float DisplayTime;
        [TextArea(2, 4)]
        public string Text;
    }
}


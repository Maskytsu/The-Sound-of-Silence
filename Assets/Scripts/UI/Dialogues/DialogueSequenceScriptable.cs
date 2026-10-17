using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSequence", menuName = "ScriptableObjects/DialogueSequence")]
public class DialogueSequenceScriptable : ScriptableObject
{
    public bool IsThoughts;
    public List<DialogueLine> DialogueLines;
    public event Action OnDialogueEnd;

    public float GetDialogueDuration()
    {
        float timeOfDialogues = 0;

        foreach (var dialogueLine in DialogueLines)
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
        public DialogueColorType ColorType;
        [TextArea(2, 4)] public string Text;

        public float ReadingTime => Text.GetReadingTime(true);

        public void HandleEvent()
        {

        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTDialogue : MonoBehaviour
{
    [SerializeField] private DialogueSequenceScriptable _dialogueSequence;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            UIDisplayManager.Instance.DisplayDialogueSequence(_dialogueSequence);
        }
    }
}
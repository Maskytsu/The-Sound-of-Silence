using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueOnPillsPickUp : MonoBehaviour
{
    [SerializeField] private DialogueSequenceScriptable _pillsDialogue;
    [SerializeField] private Pills _pills;

    private void Start()
    {
        _pills.OnInteract += () => DialogueManager.Instance.DisplayDialogue(_pillsDialogue, 0.75f);
    }
}

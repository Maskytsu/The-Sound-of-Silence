using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueOnPillsPickUp : MonoBehaviour
{
    [SerializeField] private DialogueSequenceScriptable _pillsDialogue;
    [SerializeField] private Pills _pills;

    private void Start()
    {
        _pills.OnInteract += () => StartCoroutine(DisplayDialogueDialyed());
    }

    private IEnumerator DisplayDialogueDialyed()
    {
        yield return new WaitForSeconds(0.75f);
        UIManager.Instance.DisplayDialogueSequence(_pillsDialogue);
    }
}

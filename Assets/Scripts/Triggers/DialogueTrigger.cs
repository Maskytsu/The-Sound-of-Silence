using UnityEngine;

public class DialogueTrigger : Trigger
{
    [SerializeField] private DialogueSequenceScriptable _dialogue;

    protected override void Start()
    {
        base.Start();
        OnObjectTriggerEnter += PlayDialogue;
    }

    private void PlayDialogue()
    {
        gameObject.SetActive(false);
        DialogueManager.Instance.DisplayDialogue(_dialogue);
    }
}
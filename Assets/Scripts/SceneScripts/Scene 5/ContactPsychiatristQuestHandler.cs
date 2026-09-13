using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactPsychiatristQuestHandler : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _psychiatristQuest;
    [SerializeField] private QuestScriptable _goSleepQuest;
    [SerializeField] private PhoneSetupScriptable _policeClaireInteractableSetup;
    [SerializeField] private ContactScriptable _claireInteractableContact;
    [SerializeField] private DialogueSequenceScriptable _afterNoteDialogueSequence;
    [SerializeField] private DialogueSequenceScriptable _afterContactDialogueSequence;
    [Header("Scene Objects")]
    [SerializeField] private Note _note;

    private void Start()
    {
        _note.OnFirstReadingEnd += () => StartCoroutine(StartPsychiatrisQuest());

        _claireInteractableContact.OnSendMessage += EndQuest;
        _claireInteractableContact.OnCall += EndQuest;
    }

    private IEnumerator StartPsychiatrisQuest()
    {
        //TODO: placeholder - make it as an audio xd
        DialogueManager.Instance.DisplayDialogue(_afterNoteDialogueSequence);
        yield return new WaitForSeconds(_afterNoteDialogueSequence.GetDialogueDuration());

        yield return StartCoroutine(QuestManager.Instance.StartQuestDelayed(_psychiatristQuest));
        PhoneManager.Instance.ChangePhoneSetup(_policeClaireInteractableSetup);
    }

    private void EndQuest()
    {
        _claireInteractableContact.OnSendMessage -= EndQuest;
        _claireInteractableContact.OnCall -= EndQuest;
        QuestManager.Instance.EndQuest(_psychiatristQuest);
        StartCoroutine(StartGoSleepQuest());
    }

    private IEnumerator StartGoSleepQuest()
    {
        while (PlayerObjects.Instance.PlayerEquipment.IsPhoneScreenOn)
        {
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.5f);
        DialogueManager.Instance.DisplayDialogue(_afterContactDialogueSequence);
        StartCoroutine(QuestManager.Instance.StartQuestDelayed(_goSleepQuest, 0.9f * _afterContactDialogueSequence.GetDialogueDuration()));
    }
}

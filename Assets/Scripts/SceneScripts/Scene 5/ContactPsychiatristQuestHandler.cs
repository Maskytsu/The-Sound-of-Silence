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
    [Header("Scene Objects")]
    [SerializeField] private Note _note;

    private void Start()
    {
        _note.OnFirstReadingEnd += () => StartCoroutine(StartPsychiatrisQuest());

        _claireInteractableContact.OnSendMessage += EndQuest;
        _claireInteractableContact.OnCall += EndQuest;

        _psychiatristQuest.OnQuestEnd += () => StartCoroutine(QuestManager.Instance.StartQuestDelayed(_goSleepQuest));
    }

    private IEnumerator StartPsychiatrisQuest()
    {
        yield return StartCoroutine(QuestManager.Instance.StartQuestDelayed(_psychiatristQuest));
        PhoneManager.Instance.ChangePhoneSetup(_policeClaireInteractableSetup);
    }

    private void EndQuest()
    {
        _claireInteractableContact.OnSendMessage -= EndQuest;
        _claireInteractableContact.OnCall -= EndQuest;
        QuestManager.Instance.EndQuest(_psychiatristQuest);
    }
}

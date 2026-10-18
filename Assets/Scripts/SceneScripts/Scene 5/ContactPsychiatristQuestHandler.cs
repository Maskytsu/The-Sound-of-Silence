using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactPsychiatristQuestHandler : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _psychiatristQuest;
    [SerializeField] private QuestScriptable _escapeQuest;
    [SerializeField] private QuestScriptable _goSleepQuest;
    [SerializeField] private PhoneSetupScriptable _policeClaireInteractableSetup;
    [SerializeField] private ContactScriptable _claireInteractableContact;
    [SerializeField] private DialogueSequenceScriptable _afterContactDialogueSequence;
    [Header("Scene Objects")]
    [SerializeField] private Note _note;
    [SerializeField] private PickableItem _itemKeys;

    EventInstance _breathingAudio;

    private void Start()
    {
        _note.OnFirstReadingEnd += () => StartCoroutine(StartPsychiatrisQuest());
        _note.OnFirstReadingEnd += () => QuestManager.Instance.StartQuest(_escapeQuest);
        _itemKeys.OnInteract += () => StopBreathingAudio(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        _claireInteractableContact.OnSendMessage += EndQuest;
        _claireInteractableContact.OnCall += EndQuest;
    }

    private void OnDestroy()
    {
        StopBreathingAudio(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    private IEnumerator StartPsychiatrisQuest()
    {
        _breathingAudio = RuntimeManager.CreateInstance(FmodEvents.Instance.LoopedHeartBeat);
        _breathingAudio.start();

        yield return StartCoroutine(QuestManager.Instance.StartQuestDelayed(_psychiatristQuest));
        PhoneManager.Instance.ChangePhoneSetup(_policeClaireInteractableSetup);
    }

    private void EndQuest()
    {
        StopBreathingAudio(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        _claireInteractableContact.OnSendMessage -= EndQuest;
        _claireInteractableContact.OnCall -= EndQuest;
        QuestManager.Instance.EndQuest(_psychiatristQuest);
        StartCoroutine(StartGoSleepQuest());
    }

    private IEnumerator StartGoSleepQuest()
    {
        while (PlayerObjects.Instance.PlayerEquipment.IsPhoneScreenOn || PhoneManager.Instance.IsInCall)
        {
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.5f);
        DialogueManager.DisplayDialogue(_afterContactDialogueSequence);
        StartCoroutine(QuestManager.Instance.StartQuestDelayed(_goSleepQuest, 0.9f * _afterContactDialogueSequence.GetDialogueDuration()));
    }

    private void StopBreathingAudio(FMOD.Studio.STOP_MODE mode)
    {
        _breathingAudio.stop(mode);
        _breathingAudio.release();
    }
}

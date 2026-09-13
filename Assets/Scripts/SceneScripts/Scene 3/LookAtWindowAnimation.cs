using System.Collections;
using UnityEngine;

public class LookAtWindowAnimation : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _useToiletQuest;
    [SerializeField] private DialogueSequenceScriptable _dialogueSequence;
    [SerializeField] private QuestScriptable _goSleepQuest;
    [Header("Scene Objects")]
    [SerializeField] private Window _window;
    [SerializeField] private Trigger _lookAtWindowTrigger;

    private void Start()
    {
        _useToiletQuest.OnQuestEnd += () =>
        {
            _lookAtWindowTrigger.gameObject.SetActive(true);
            _window.InstantOpenWindow();
        };

        _lookAtWindowTrigger.OnObjectTriggerEnter += () =>
        {
            _lookAtWindowTrigger.gameObject.SetActive(false);
            StartCoroutine(LookAtWindow());
        };
    }

    private IEnumerator LookAtWindow()
    {
        yield return StartCoroutine(CameraManager.Instance.LookAtTargetAnimation(_window.InteractionHitbox.transform, lookingAtTargetTime: 0.5f));

        DialogueManager.Instance.DisplayDialogue(_dialogueSequence);
        yield return new WaitForSeconds(0.5f * _dialogueSequence.GetDialogueDuration());

        InputProvider.Instance.TurnOnPlayerMaps();
        QuestManager.Instance.StartQuest(_goSleepQuest);
    }
}

using System.Collections;
using UnityEngine;

public class LookAtWindowAnimation : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _useToiletQuest;
    [SerializeField] private QuestScriptable _goSleepQuest;
    [Header("Scene Objects")]
    [SerializeField] private Window _window;
    [SerializeField] private PlayerTrigger _lookAtWindowTrigger;

    private void Start()
    {
        _useToiletQuest.OnQuestEnd += () =>
        {
            _lookAtWindowTrigger.gameObject.SetActive(true);
            _window.OpenWindow();
        };

        _lookAtWindowTrigger.OnPlayerTriggerEnter += () =>
        {
            _lookAtWindowTrigger.gameObject.SetActive(false);
            StartCoroutine(LookAtWindow());
        };
    }

    private IEnumerator LookAtWindow()
    {
        yield return StartCoroutine(CameraManager.Instance.LookAtTargetAnimation(_window.transform));

        QuestManager.Instance.StartQuest(_goSleepQuest);
    }
}

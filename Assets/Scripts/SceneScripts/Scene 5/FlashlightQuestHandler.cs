using UnityEngine;

public class FlashlightQuestHandler : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _flashlightQuest;
    [Header("Scene Objects")]
    [SerializeField] private ThunderWakeUpSequence _wakeUpSequence;
    [SerializeField] private PickableItem _flashlight;

    private void Start()
    {
        _wakeUpSequence.OnAnimationEnd += () => QuestManager.Instance.StartQuest(_flashlightQuest);

        _flashlight.OnInteract += () => QuestManager.Instance.EndQuest(_flashlightQuest);
    }
}

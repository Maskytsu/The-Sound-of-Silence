using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeysQuestHandler : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _keysQuest;
    [Header("Scene Objects")]
    [SerializeField] private AfterHospitalWakeUpSequence _wakeUpSequence;
    [SerializeField] private PickableItem _keys;

    private void Start()
    {
        _wakeUpSequence.OnAnimationEnd += () => QuestManager.Instance.StartQuest(_keysQuest);

        _keys.OnInteract += () => QuestManager.Instance.EndQuest(_keysQuest);
    }
}

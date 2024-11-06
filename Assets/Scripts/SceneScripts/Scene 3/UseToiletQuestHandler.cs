using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseToiletQuestHandler : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _useToiletQuest;
    [Header("Scene Objects")]
    [SerializeField] private WakeUpSequence _wakeUpSeq;

    private void Start()
    {
        _wakeUpSeq.OnAnimationEnd += () => QuestManager.Instance.StartQuest(_useToiletQuest);
    }
}

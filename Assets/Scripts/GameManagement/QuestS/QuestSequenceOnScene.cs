using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSequenceOnScene : MonoBehaviour
{
    [SerializeField] private List<QuestScriptable> _quests;

    private Dictionary<QuestScriptable, QuestScriptable> _questQueue;

    private void Awake()
    {
        _questQueue = new Dictionary<QuestScriptable, QuestScriptable>();

        for (int i = 0; i < _quests.Count - 1; i++)
        {
            _questQueue.Add(_quests[i], _quests[i + 1]);
            _quests[i].OnQuestEnd += StartNextQuest;
        }
    }

    private void StartNextQuest(QuestScriptable quest)
    {
        StartCoroutine(DisplayQuestDelayed(2f, _questQueue[quest]));
    }

    private IEnumerator DisplayQuestDelayed(float delayTime, QuestScriptable quest)
    {
        yield return new WaitForSeconds(delayTime);
        quest.StartQuest();
        UIDisplayManager.Instance.DisplayNewQuest(quest);
    }
}

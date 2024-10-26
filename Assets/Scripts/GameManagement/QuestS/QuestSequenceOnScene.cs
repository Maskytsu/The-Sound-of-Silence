using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSequenceOnScene : MonoBehaviour
{
    [SerializeField] private List<QuestScriptable> _quests;

    private int _currentQuestIndex = 0;

    private void Awake()
    {
        for (int i = 0; i < _quests.Count - 1; i++)
        {
            _quests[i].OnQuestEnd += StartNextQuest;
        }
    }

    private void OnDestroy()
    {
        foreach(var quest in _quests)
        {
            quest.ClearSubscribers();
        }
    }

    private void StartNextQuest(QuestScriptable quest)
    {
        _currentQuestIndex++;
        StartCoroutine(DisplayQuestDelayed(2f, _quests[_currentQuestIndex]));
    }

    private IEnumerator DisplayQuestDelayed(float delayTime, QuestScriptable quest)
    {
        yield return new WaitForSeconds(delayTime);
        quest.StartQuest();
        UIManager.Instance.DisplayNewQuest(quest);
    }
}

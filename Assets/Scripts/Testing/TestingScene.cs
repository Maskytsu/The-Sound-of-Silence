using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class TestingScene : MonoBehaviour
{
    [SerializeField, ReorderableList] private List<QuestScriptable> _quests;

    [Button]
    private void StartFirstQuestFromList()
    {
        QuestManager.Instance.StartQuest(_quests[0]);
    }

    [Button]
    private void EndFirstQuestFromList()
    {
        QuestManager.Instance.EndQuest(_quests[0]);
    }
}

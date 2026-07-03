using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestingScene : MonoBehaviour
{
    [SerializeField, ReorderableList] private List<QuestScriptable> _quests;
    [Space]
    [SerializeField] private GameObject _testPrefab;

    [Button]
    private void SpawnTestPrefab()
    {
        Instantiate(_testPrefab);
    }

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

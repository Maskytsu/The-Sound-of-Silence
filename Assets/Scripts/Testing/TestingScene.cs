using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestingScene : MonoBehaviour
{
    [SerializeField, ReorderableList] private List<QuestScriptable> _quests;
    [SerializeField] private bool _unlockLeap = true;
    [Space]
    [SerializeField] private GameObject _testPrefab;

    private void Start()
    {
        if (_unlockLeap)
        {
            GameState.Instance.LeapUnlocked = true;
        }
    }

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

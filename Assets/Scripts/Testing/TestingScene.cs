using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class TestingScene : MonoBehaviour
{
    [SerializeField, ReorderableList] private List<QuestScriptable> _quests;

    private void Start()
    {
        AssignDoorEvents();
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

    [HorizontalLine]
    [SerializeField] private Door _door1;
    [SerializeField] private Door _door2;

    private void AssignDoorEvents()
    {
        _door1.InteractionHitbox.OnInteract += _door2.SwitchDoorAnimated;
        _door2.InteractionHitbox.OnInteract += _door1.SwitchDoorAnimated;
    }
}

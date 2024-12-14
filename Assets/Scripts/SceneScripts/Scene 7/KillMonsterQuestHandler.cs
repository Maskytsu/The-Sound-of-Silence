using UnityEngine;
using NaughtyAttributes;

public class KillMonsterQuestHandler : MonoBehaviour
{
    [ReadOnly] public bool MonsterKilled;

    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _killItQuest;
    [Header("Scene Objects")]
    [SerializeField] private MonsterStateMachine _monsterStateMachine;

    private void Start()
    {
        MonsterKilled = false;
        _monsterStateMachine.OnMonsterKilled += ManageMonsterKilled;
    }

    private void ManageMonsterKilled()
    {
        MonsterKilled = true;
        QuestManager.Instance.EndQuest(_killItQuest);
    }
}

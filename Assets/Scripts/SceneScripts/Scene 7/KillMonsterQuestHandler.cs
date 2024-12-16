using UnityEngine;
using NaughtyAttributes;

public class KillMonsterQuestHandler : MonoBehaviour
{
    [ReadOnly] public bool MonsterKilled;
    [ReadOnly] public bool QuestEnded;

    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _killItQuest;
    [Header("Scene Objects")]
    [SerializeField] private MonsterStateMachine _monsterStateMachine;

    private void Start()
    {
        MonsterKilled = false;
        _monsterStateMachine.OnMonsterKilled += ManageMonsterKilled;
    }

    public void FailQuest()
    {
        if (QuestEnded)
        {
            Debug.LogError("Quest already ended!");
            return;
        }

        if (MonsterKilled)
        {
            Debug.LogError("Monster was killed, so quest isn't failed!");
            return;
        }

        //player didn't take a gun but went into safe room
        //or player left hospital and didn't kill monster
        QuestEnded = true;
        QuestManager.Instance.EndQuest(_killItQuest);
    }

    private void ManageMonsterKilled()
    {
        MonsterKilled = true;
        QuestEnded = true;
        QuestManager.Instance.EndQuest(_killItQuest);
    }
}

using UnityEngine;
using NaughtyAttributes;

public class KillMonsterQuestHandler : MonoBehaviour
{
    [ReadOnly] public bool MonsterKilled;
    [ReadOnly] public bool QuestEnded;

    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _killItQuest;
    [SerializeField] private QuestScriptable _escapeQuest;
    [Header("Scene Objects")]
    [SerializeField] private MonsterStateMachine _monsterStateMachine;
    [SerializeField] private FenceGateLock _roadFenceGateLock;
    [SerializeField] private MonsterSwappingStairsAnimation _shedStairsAnimation;
    [SerializeField] private ResetingBreakersHandler _resetingBreakersHandler;

    private void Start()
    {
        MonsterKilled = false;

        _monsterStateMachine.OnMonsterKilled += ManageMonsterKilled;
        _monsterStateMachine.OnMonsterKilled += _shedStairsAnimation.SkipAnimation;
        _monsterStateMachine.OnMonsterKilled += _resetingBreakersHandler.InstantTeleportBasement;
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

        QuestManager.Instance.EndQuest(_escapeQuest);
        _roadFenceGateLock.InteractableHitbox.gameObject.SetActive(false);
        _roadFenceGateLock.UnlockableHitbox.gameObject.SetActive(false);
    }
}

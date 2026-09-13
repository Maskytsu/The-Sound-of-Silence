using UnityEngine;
using NaughtyAttributes;

public class KillMonsterQuestHandler : MonoBehaviour
{
    [ReadOnly] public bool MonsterKilled;
    [ReadOnly] public bool QuestEnded;

    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _killItQuest;
    [SerializeField] private QuestScriptable _escapeQuest;
    [SerializeField] private DialogueSequenceScriptable _shotgunPickUpDialogue;
    [SerializeField] private DialogueSequenceScriptable _monsterKilledDialogue;
    [Header("Scene Objects")]
    [SerializeField] private FenceGateLock _roadFenceGateLock;
    [SerializeField] private PickableItem _shotgun;
    [SerializeField] private MonsterSwappingStairsAnimation _shedStairsAnimation;
    [SerializeField] private ResetingBreakersHandler _resetingBreakersHandler;

    private void Start()
    {
        MonsterKilled = false;

        var monsterSM = MonsterStateMachine.Instance;
        monsterSM.OnMonsterKilled += ManageMonsterKilled;
        monsterSM.OnMonsterKilled += _shedStairsAnimation.SkipAnimation;
        monsterSM.OnMonsterKilled += _resetingBreakersHandler.InstantTeleportBasement;
        _shotgun.OnInteract += () => DialogueManager.Instance.DisplayDialogue(_shotgunPickUpDialogue, 0.5f);
    }

    public void FailQuest(bool questWasStarted = true)
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
        if (questWasStarted) QuestManager.Instance.EndQuest(_killItQuest);
    }

    private void ManageMonsterKilled()
    {
        MonsterKilled = true;
        QuestEnded = true;
        QuestManager.Instance.EndQuest(_killItQuest);
        QuestManager.Instance.EndQuest(_escapeQuest);

        DialogueManager.Instance.DisplayDialogue(_monsterKilledDialogue, 2.0f);

        _roadFenceGateLock.InteractableHitbox.gameObject.SetActive(false);
        _roadFenceGateLock.UnlockableHitbox.gameObject.SetActive(false);
    }
}

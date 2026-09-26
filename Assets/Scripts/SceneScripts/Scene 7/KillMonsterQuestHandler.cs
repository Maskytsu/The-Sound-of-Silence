using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField] private CatchingHandler _catchingHandler;
    [SerializeField] private PlayerTargetTransform _playerTargetTransform;

    private BlinkEffect Blink => HUD.Instance.Blink;

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

        _roadFenceGateLock.InteractableHitbox.gameObject.SetActive(false);
        _roadFenceGateLock.UnlockableHitbox.gameObject.SetActive(false);

        if (_catchingHandler.AnyCheckpointReached)
        {
            StartCoroutine(TeleportPlayerIfDreaming());
        }
        else
        {
            DialogueManager.Instance.DisplayDialogue(_monsterKilledDialogue, 4.0f);
        }
    }

    private IEnumerator TeleportPlayerIfDreaming()
    {
        yield return new WaitForSeconds(4f);

        InputProvider.Instance.TurnOffGameplayOverlayMap();

        Blink.PlayCloseEyes(3.0f);

        yield return null;
        while (Blink.IsPlaying) yield return null;
        PlayerObjects.Instance.PlayerMovement.SetTransformInstant(_playerTargetTransform, true);
        yield return new WaitForSeconds(3f);

        Blink.PlayOpenEyes(3.0f);
        yield return null;
        while (Blink.IsPlaying) yield return null;
        yield return new WaitForSeconds(1f);

        InputProvider.Instance.TurnOnGameplayMaps();
        DialogueManager.Instance.DisplayDialogue(_monsterKilledDialogue, 3.0f);
    }
}

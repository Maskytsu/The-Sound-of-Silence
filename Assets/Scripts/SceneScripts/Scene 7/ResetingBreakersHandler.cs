using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetingBreakersHandler : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _breakersQuest;
    [SerializeField] private QuestScriptable _couchQuest;
    [SerializeField] private QuestScriptable _bedQuest;
    [SerializeField] private DialogueSequenceScriptable _hearingAidDialogue;

    [Header("Scene Objects")]
    [SerializeField] private KillMonsterQuestHandler _killQuestHandler;
    [SerializeField] private Door _shedDoor;
    [SerializeField] private DialogueInteracion _shedDoorDialogue;
    [SerializeField] private Breakers _breakers;
    [SerializeField] private HearingAid _hearingAid;
    [Space]
    [SerializeField] private GameObject _stairsLong;
    [SerializeField] private GameObject _stairsShort;
    [Space]
    [SerializeField] private Transform _basement;
    [SerializeField] private Transform _basementTargetPos;

    [Header("Exit Dialogue")]
    [SerializeField] private Trigger _dialogueTrigger;
    [SerializeField] private DialogueSequenceScriptable _monsterKilledDialogue;
    [SerializeField] private DialogueSequenceScriptable _goodEndingDialogue;
    [SerializeField] private DialogueSequenceScriptable _badEndingDialogue;

    private bool _breakersReseted = false;

    private void Start()
    {
        _breakers.OnInteract += HandleResetingBreakers;

        _breakers.OnInteract += EnableDoorIfBothInteracted;
        _hearingAid.OnInteract += EnableDoorIfBothInteracted;

        _hearingAid.OnInteract += () => DialogueManager.Instance.DisplayDialogue(_hearingAidDialogue, 0.5f);
    }

    public void InstantTeleportBasement()
    {
        _stairsLong.SetActive(false);
        _stairsShort.SetActive(true);

        _basement.position = _basementTargetPos.position;
    }

    private void HandleResetingBreakers()
    {
        _breakersReseted = true;
        QuestManager.Instance.EndQuest(_breakersQuest);

        if (_hearingAid.gameObject.activeSelf)
        {
            _shedDoorDialogue.InteractionHitbox.gameObject.SetActive(true);
        }

        HandleDialogue();
        StartCoroutine(TeleportRoomAndPlayer());
    }

    private void EnableDoorIfBothInteracted()
    {
        //_hearingAid not interacted or _breakers not interacted
        if (_hearingAid.gameObject.activeSelf || !_breakersReseted)
        {
            _shedDoor.SetOpened(false);
            _shedDoor.InteractionHitbox.gameObject.SetActive(false);
            return;
        }

        _shedDoor.InteractionHitbox.gameObject.SetActive(true);
        _shedDoorDialogue.InteractionHitbox.gameObject.SetActive(false);
    }

    private IEnumerator TeleportRoomAndPlayer()
    {
        _stairsLong.SetActive(false);
        _stairsShort.SetActive(true);

        //tp player and room
        PlayerObjects.Instance.Player.transform.parent = _basement;
        _basement.position = _basementTargetPos.position;
        PlayerObjects.Instance.Player.transform.parent = null;

        //turn off character controller for one frame because it breaks teleportation if wants to move
        CharacterController playerCharacterController = PlayerObjects.Instance.Player.GetComponent<CharacterController>();
        playerCharacterController.enabled = false;
        yield return null;
        playerCharacterController.enabled = true;
    }

    private void HandleDialogue()
    {
        _dialogueTrigger.gameObject.SetActive(true);

        if (_killQuestHandler.MonsterKilled)
        {
            _dialogueTrigger.OnObjectTriggerEnter += () => {
                DialogueManager.Instance.DisplayDialogue(_monsterKilledDialogue);
                StartCoroutine(QuestManager.Instance.StartQuestDelayed(_couchQuest)); 
            };
            return;
        }

        _dialogueTrigger.OnObjectTriggerEnter += () => {
            DialogueManager.Instance.DisplayDialogue(GameState.Instance.HasRequirementsForGoodEnding ? _goodEndingDialogue : _badEndingDialogue);
            StartCoroutine(QuestManager.Instance.StartQuestDelayed(_bedQuest));
        };
    }
}
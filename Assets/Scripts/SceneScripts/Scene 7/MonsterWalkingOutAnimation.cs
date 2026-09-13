using DG.Tweening;
using UnityEngine;

public class MonsterWalkingOutAnimation : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _escapeQuest;
    [SerializeField] private QuestScriptable _killItQuest;
    [SerializeField] private DialogueSequenceScriptable _dialogueSequence;
    [Header("Scene Objects")]
    [SerializeField] private Trigger _monsterWalkOutTrigger;
    [SerializeField] private Trigger _blockPlayerTrigger;
    [SerializeField] private Transform _monster;
    [SerializeField] private Transform _monsterTargetPos;
    [SerializeField] private Scene7ResetHandler _sceneResetHandler;
    [Space]
    [SerializeField] private float _monsterSpeed = 3f;

    private void Start()
    {
        if (_sceneResetHandler.SceneWasResetedByCatch)
        {
            CancelAnimation();
            return;
        }

        _monsterWalkOutTrigger.OnObjectTriggerEnter += MonsterAnimation;
        _blockPlayerTrigger.OnObjectTriggerEnter += InputProvider.Instance.TurnOffPlayerMovementMap;
    }

    private void MonsterAnimation()
    {
        _monster.DOMove(_monsterTargetPos.position, _monsterSpeed).SetSpeedBased().SetEase(Ease.Linear).onComplete += () =>
        {
            DialogueManager.Instance.DisplayDialogue(_dialogueSequence);

            Destroy(_monster.gameObject);
            InputProvider.Instance.TurnOnPlayerMovementMap();

            MonsterStateMachine.Instance.gameObject.SetActive(true);
            GameState.Instance.LeapUnlocked = true;
        };

        StartCoroutine(QuestManager.Instance.StartQuestDelayed(_escapeQuest, 11f));
        StartCoroutine(QuestManager.Instance.StartQuestDelayed(_killItQuest, 5f));
    }

    private void CancelAnimation()
    {
        Destroy(_monster.gameObject);
        MonsterStateMachine.Instance.gameObject.SetActive(true);

        StartCoroutine(QuestManager.Instance.StartQuestDelayed(_escapeQuest, 31f));
        StartCoroutine(QuestManager.Instance.StartQuestDelayed(_killItQuest, 25f));
    }
}

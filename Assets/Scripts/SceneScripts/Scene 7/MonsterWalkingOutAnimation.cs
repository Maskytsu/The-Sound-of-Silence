using DG.Tweening;
using UnityEngine;

public class MonsterWalkingOutAnimation : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _escapeQuest;
    [SerializeField] private QuestScriptable _killItQuest;
    [Header("Scene Objects")]
    [SerializeField] private Trigger _monsterWalkOutTrigger;
    [SerializeField] private Transform _monster;
    [SerializeField] private Transform _monsterTargetPos;
    [SerializeField] private Scene7ResetHandler _sceneResetHandler;
    [Space]
    [SerializeField] private float _monsterSpeed = 3.5f;

    private void Start()
    {
        if (!_sceneResetHandler.SceneWasResetedByCatch) _monsterWalkOutTrigger.OnObjectTriggerEnter += MonsterAnimation;
        else CancelAnimation();
    }

    private void MonsterAnimation()
    {
        _monsterWalkOutTrigger.gameObject.SetActive(false);

        _monster.DOMove(_monsterTargetPos.position, _monsterSpeed).SetSpeedBased().SetEase(Ease.Linear).onComplete += () =>
        {
            Destroy(_monster.gameObject);
            MonsterStateMachine.Instance.gameObject.SetActive(true);
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

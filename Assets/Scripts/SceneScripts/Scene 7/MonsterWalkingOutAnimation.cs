using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterWalkingOutAnimation : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _escapeQuest;
    [SerializeField] private QuestScriptable _killItQuest;
    [Header("Scene Objects")]
    [SerializeField] private Door _door;
    [SerializeField] private Transform _monster;
    [SerializeField] private MonsterStateMachine _monsterStateMachine;
    [SerializeField] private Scene7ResetHandler _sceneResetHandler;
    [Header("Parameters")]
    [SerializeField] private float _monsterTargetZPosition;

    private void Start()
    {
        if (!_sceneResetHandler.SceneWasReseted) _door.OnInteract += MonsterAnimation;
        else CancelAnimation();
    }

    private void MonsterAnimation()
    {
        _door.OnInteract -= MonsterAnimation;

        _monster.DOMoveZ(_monsterTargetZPosition, 3f).SetSpeedBased().SetEase(Ease.Linear).onComplete += () =>
        {
            Destroy(_monster.gameObject);
            _monsterStateMachine.gameObject.SetActive(true);
        };

        StartCoroutine(QuestManager.Instance.StartQuestDelayed(_escapeQuest, 11f));
        StartCoroutine(QuestManager.Instance.StartQuestDelayed(_killItQuest, 5f));
    }

    private void CancelAnimation()
    {
        Destroy(_monster.gameObject);
        _monsterStateMachine.gameObject.SetActive(true);

        StartCoroutine(QuestManager.Instance.StartQuestDelayed(_escapeQuest, 31f));
        StartCoroutine(QuestManager.Instance.StartQuestDelayed(_killItQuest, 25f));
    }
}

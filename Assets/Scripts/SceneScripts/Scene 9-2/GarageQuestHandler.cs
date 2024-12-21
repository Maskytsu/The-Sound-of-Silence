using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarageQuestHandler : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _garageQuest;
    [SerializeField] private QuestScriptable _forestQuest;
    [Header("Scene Objects")]
    [SerializeField] private Rope _rope;
    [SerializeField] private GameObject _monsterHouse;
    [SerializeField] private GameObject _outsideMonster;
    [SerializeField] private Door _garageDoor;
    [SerializeField] private Door _houseExitDoor;
    [SerializeField] private FenceGate _fenceGate;

    private void Start()
    {
        _rope.OnInteract += EndGarageQuest;
    }

    private void EndGarageQuest()
    {
        QuestManager.Instance.EndQuest(_garageQuest);
        StartCoroutine(QuestManager.Instance.StartQuestDelayed(_forestQuest));

        _houseExitDoor.InteractionHitbox.gameObject.SetActive(true);
        _houseExitDoor.OnInteract += TeleportMonster;
    }

    private void TeleportMonster()
    {
        _monsterHouse.SetActive(false);
        _outsideMonster.SetActive(true);

        _garageDoor.SetOpened(false);
        _fenceGate.SetOpened(true);
    }
}

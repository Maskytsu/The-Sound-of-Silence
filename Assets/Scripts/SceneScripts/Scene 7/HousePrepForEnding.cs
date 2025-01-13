using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousePrepForEnding : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _escapeQuest;
    [Header("Scene Objects")]
    [SerializeField] private Breakers _breakers;
    [SerializeField] private KillMonsterQuestHandler _killQuestHandler;
    [Space]
    [SerializeField] private GameObject _imaginedHarryRoom;
    [SerializeField] private GameObject _realHarryRoom;
    [SerializeField] private GameObject _sharonRoomSmallLight;
    [SerializeField] private Door _houseEntryDoor;
    [Space]
    [SerializeField] private MirrorMonsterAnimation _mirrorAnimation;
    [SerializeField] private FenceGateLock _roadFenceGetLock;
    [SerializeField] private FenceGate _roadFenceGate;

    private void Start()
    {
        _breakers.OnInteract += PrepareHouse;
    }

    private void PrepareHouse()
    {
        _houseEntryDoor.InteractionHitbox.gameObject.SetActive(true);

        _mirrorAnimation.TurnOffAnimation();

        _roadFenceGetLock.UnlockableHitbox.gameObject.SetActive(false);
        _roadFenceGetLock.InteractableHitbox.gameObject.SetActive(false);
        _roadFenceGate.SetOpened(false);

        if (!_killQuestHandler.MonsterKilled) QuestManager.Instance.EndQuest(_escapeQuest);

        _imaginedHarryRoom.SetActive(false);
        _realHarryRoom.SetActive(true);

        _sharonRoomSmallLight.SetActive(true);
    }
}
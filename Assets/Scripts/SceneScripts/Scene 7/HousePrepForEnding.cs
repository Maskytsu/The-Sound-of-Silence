using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousePrepForEnding : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _escapeQuest;
    [Header("Scene Objects")]
    [SerializeField] private Breakers _breakers;
    [Space]
    [SerializeField] private List<GameObject> _imaginedHarryRoom;
    [SerializeField] private List<GameObject> _realHarryRoom;
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

        QuestManager.Instance.EndQuest(_escapeQuest);

        SetActiveObjects(_imaginedHarryRoom, false);
        SetActiveObjects(_realHarryRoom, true);

        _sharonRoomSmallLight.SetActive(true);
    }


    private void SetActiveObjects(List<GameObject> gObjects, bool activeStateToSet)
    {
        foreach (GameObject gObject in gObjects)
        {
            gObject.SetActive(activeStateToSet);
        }
    }
}
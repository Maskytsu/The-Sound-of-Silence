using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetingBreakersHandler : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _breakersQuest;
    [Header("Scene Objects")]
    [SerializeField] private Door _shedDoor;
    [SerializeField] private Breakers _breakers;
    [SerializeField] private HearingAid _hearingAid;
    [Space]
    [SerializeField] private GameObject _stairsLong;
    [SerializeField] private GameObject _stairsShort;
    [Space]
    [SerializeField] private Transform _basement;
    [SerializeField] private Transform _basementTargetPos;

    private bool _breakersReseted = false;

    private void Start()
    {
        _breakers.OnInteract += HandleResetingBreakers;
        _hearingAid.OnInteract += EnableDoorIfBothInteracted;
    }

    private void HandleResetingBreakers()
    {
        _breakersReseted = true;

        QuestManager.Instance.EndQuest(_breakersQuest);

        StartCoroutine(TeleportRoomAndPlayer());

        EnableDoorIfBothInteracted();
    }

    private void EnableDoorIfBothInteracted()
    {
        if (_hearingAid.gameObject.activeSelf || !_breakersReseted) return;

        _shedDoor.SetOpened(true);
        _shedDoor.InteractionHitbox.gameObject.SetActive(true);
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
}
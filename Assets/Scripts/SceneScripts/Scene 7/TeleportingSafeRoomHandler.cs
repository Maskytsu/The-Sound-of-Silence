using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportingSafeRoomHandler : MonoBehaviour
{
    public MonsterStateMachine MonsterSM => _monsterSM;
    public TeleportingChosenMonsterState TpChosenState => _tpChosenState;
    public int TpDirectionIndex => _tpDirectionIndex;
    public List<Transform> NewPatrolingPoints => _newPatrolingPoints;

    [Header("Activation")]
    [SerializeField] private PickableItem _keys;
    [SerializeField] private PortalCamera _portalCameraHandler;
    [SerializeField] private Transform _portalScreen;
    [Header("Close/Far changes")]
    [SerializeField] private Trigger _playerCloseTrigger;
    [SerializeField] private Terrain _terrain;
    [SerializeField] private List<GameObject> _objectsInTheWay;
    [Header("Room Teleportation")]
    [SerializeField] private Trigger _playerCloseDoorTrigger;
    [SerializeField] private Door _houseDoor;
    [SerializeField] private Door _safeRoomEntryDoor;
    [SerializeField] private Door _safeRoomExitDoor;
    [SerializeField] private GameObject _doorBlockade;
    [SerializeField] private Transform _safeRoom;
    [SerializeField] private Transform _safeRoomTargetPos;
    [SerializeField] private StormEffect _storm;
    [SerializeField] private KillMonsterQuestHandler _killMonsterQuestHandler;
    [Header("Monster Teleportation")]
    [SerializeField] private MonsterStateMachine _monsterSM; //can be null if killed
    [SerializeField] private TeleportingChosenMonsterState _tpChosenState;
    [SerializeField] private int _tpDirectionIndex = 0;
    [SerializeField] private List<Transform> _newPatrolingPoints;

    private float _savedDetailDistance;
    private bool _shouldCheckDoor = false;

    private void Start()
    {
        _keys.OnInteract += StartPortalEffect;
        _keys.OnInteract += TeleportMonster;

        _playerCloseTrigger.OnObjectTriggerEnter += SetCloseView;
        _playerCloseTrigger.OnObjectTriggerExit += SetFarView;

        _playerCloseDoorTrigger.OnObjectTriggerEnter += CloseDoor;
    }

    private void Update()
    {
        CheckIfDoorClosed();
    }

    private void StartPortalEffect()
    {
        _savedDetailDistance = _terrain.detailObjectDistance;

        _playerCloseTrigger.gameObject.SetActive(true);
        _playerCloseDoorTrigger.gameObject.SetActive(true);

        _portalCameraHandler.DisplayPortal = true;
        _portalScreen.gameObject.SetActive(true);
    }

    private void SetCloseView()
    {
        _portalCameraHandler.IsPlayerClose = true;
        _portalScreen.gameObject.SetActive(false);

        _safeRoom.gameObject.SetActive(true);

        SetActiveObjects(_objectsInTheWay, false);
        _terrain.detailObjectDistance = 0;
    }

    private void SetFarView()
    {
        _portalCameraHandler.IsPlayerClose = false;
        _portalScreen.gameObject.SetActive(true);

        _safeRoom.gameObject.SetActive(false);

        SetActiveObjects(_objectsInTheWay, true);
        _terrain.detailObjectDistance = _savedDetailDistance;
    }

    private void TeleportMonster()
    {

        if (_monsterSM == null)
        {
            Debug.LogWarning("Monster is null. Was it killed?");
            return;
        }

        _monsterSM.ChangePatrolingPoints(_newPatrolingPoints);
        _tpChosenState.SetUpDestination(_tpDirectionIndex);
        _monsterSM.ChangeState(_tpChosenState);
    }

    private void CloseDoor()
    {
        _playerCloseDoorTrigger.gameObject.SetActive(false);

        _playerCloseTrigger.OnObjectTriggerEnter -= SetCloseView;
        _playerCloseTrigger.OnObjectTriggerExit -= SetFarView;
        _playerCloseTrigger.gameObject.SetActive(false);

        //this is potentially bugged - if door in switch animation (closed -> opened) it wont work
        //closeDoorTrigger must be placed in the correct position
        //closing door must push player into it before end of animation - befor the could open it
        if (_houseDoor.IsOpened) _houseDoor.SwitchDoorAnimated();
        _houseDoor.InteractionHitbox.gameObject.SetActive(false);
        _doorBlockade.SetActive(true);

        _shouldCheckDoor = true;
    }

    private void CheckIfDoorClosed()
    {
        if (_shouldCheckDoor)
        {
            if (!_houseDoor.IsOpened)
            {
                _shouldCheckDoor = false;
                StartCoroutine(TeleportRoomAndPlayer());
            }
        }
    }

    private IEnumerator TeleportRoomAndPlayer()
    {
        //activate exit door hitbox and visuals for entry door
        _safeRoomEntryDoor.gameObject.SetActive(true);
        _safeRoomExitDoor.InteractionHitbox.gameObject.SetActive(true);

        //tp player and room
        CharacterController playerCharacterController = PlayerObjects.Instance.Player.GetComponent<CharacterController>();
        playerCharacterController.enabled = false;
        PlayerObjects.Instance.Player.transform.parent = _safeRoom;
        _safeRoom.position = _safeRoomTargetPos.position;
        PlayerObjects.Instance.Player.transform.parent = null;
        yield return null;
        //turn off character controller for one frame because it breaks teleportation if wants to move
        playerCharacterController.enabled = true;
        //turn off storm
        _storm.OnLightningEnd += () => _storm.gameObject.SetActive(false);

        //sets house as it was without the room, triggers and portal
        _portalCameraHandler.DisplayPortal = false;
        _portalScreen.gameObject.SetActive(false);

        _doorBlockade.SetActive(false);

        SetActiveObjects(_objectsInTheWay, true);
        _terrain.detailObjectDistance = _savedDetailDistance;

        if (!ItemManager.Instance.HaveGun) _killMonsterQuestHandler.FailQuest();
    }

    private void SetActiveObjects(List<GameObject> gObjects, bool activeStateToSet)
    {
        foreach (GameObject gObject in gObjects)
        {
            gObject.SetActive(activeStateToSet);
        }
    }
}
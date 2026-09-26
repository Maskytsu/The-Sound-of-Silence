using System;
using System.Collections.Generic;
using UnityEngine;

public class TeleportingLastRoom : MonoBehaviour
{
    public event Action<TeleportingLastRoom> OnTeleportPlayer;
    public event Action<TeleportingLastRoom> OnOutsideDoorOpened;
    public event Action OnTpMonsterToRoom;
    public event Action OnCloseOutsideDoor;
    public event Action OnRoomReset;

    [SerializeField] private Trigger _lastSafeRoomCloseExitDoorTrigger;
    [SerializeField] private Door _lastSafeRoomExitDoor;
    [Header("Open Outside Door")]
    [SerializeField] private Trigger _openOutsideDoorTrigger;
    [SerializeField] private Door _outsideRoomDoor;

    [Header("TP Monster There")]
    [SerializeField] private Trigger _tpMonsterThereTrigger;
    [SerializeField] private Transform _monsterTpPos;

    [Header("Close Outside Door")]
    [SerializeField] private Trigger _closeOutsideDoorTrigger;
    [SerializeField] private GameObject _outsideDoorBlockade;

    private bool _shouldCheckLastSafeRoomDoor = false;
    private bool _shouldCheckOutsideDoor = false;

    private Vector3 _lastRoomBasePos;

    private void Start()
    {
        _lastRoomBasePos = transform.position;

        _lastSafeRoomCloseExitDoorTrigger.OnObjectTriggerEnter += () => _shouldCheckLastSafeRoomDoor = true;
        _openOutsideDoorTrigger.OnObjectTriggerEnter += OpenOutsideDoor;
        _tpMonsterThereTrigger.OnObjectTriggerEnter += TpMonsterThere;
        _closeOutsideDoorTrigger.OnObjectTriggerEnter += CloseOutsideDoor;
    }

    private void Update()
    {
        if (_shouldCheckLastSafeRoomDoor) CheckLastSafeRoomDoor();
        if (_shouldCheckOutsideDoor) CheckOutsideDoor();
    }

    private void CheckLastSafeRoomDoor()
    {
        if (!_lastSafeRoomExitDoor.IsOpened)
        {
            _lastSafeRoomCloseExitDoorTrigger.gameObject.SetActive(false);
            _shouldCheckLastSafeRoomDoor = false;
            OnTeleportPlayer?.Invoke(this);
        }
    }

    private void CheckOutsideDoor()
    {
        if (!_outsideRoomDoor.IsOpened)
        {
            _shouldCheckOutsideDoor = false;
            OnOutsideDoorOpened?.Invoke(this);
        }
    }

    private void OpenOutsideDoor()
    {
        _openOutsideDoorTrigger.gameObject.SetActive(false);

        if (_outsideRoomDoor.IsOpened)
        {
            Debug.LogWarning("Door is already open for some reason!");
            return;
        }

        _outsideRoomDoor.SwitchDoorAnimated();
    }

    private void TpMonsterThere()
    {
        _tpMonsterThereTrigger.gameObject.SetActive(false);
        OnTpMonsterToRoom?.Invoke();

        var monsterSM = MonsterStateMachine.Instance;
        if (monsterSM == null)
        {
            Debug.LogWarning("Monster is null. Was it killed?");
            return;
        }

        var tpChosenState = monsterSM.GetMonsterState<TeleportingChosenMonsterState>();
        tpChosenState.SetUpDestination(_monsterTpPos.position, true);
        monsterSM.ChangeState(tpChosenState);

        tpChosenState.OnTpDestinationReached += StartChasingPlayer;

        monsterSM.GetMonsterState<CatchingPlayerMonsterState>().OnPlayerCatched += ResetRoom;
    }

    private void StartChasingPlayer()
    {
        var monsterSM = MonsterStateMachine.Instance;
        monsterSM.ChangeState<LookingForPlayerMonsterState>();
        AudioManager.PlayOneShotOccludedRI(FmodEvents.Instance.OCC_MonsterAngry, monsterSM.gameObject, false);
    }

    private void CloseOutsideDoor()
    {
        _closeOutsideDoorTrigger.gameObject.SetActive(false);
        _outsideDoorBlockade.SetActive(true);

        if (!_outsideRoomDoor.IsOpened)
        {
            Debug.LogWarning("Door is already closed for some reason!");
        }
        else
        {
            _outsideRoomDoor.SwitchDoorAnimated();
        }

        OnCloseOutsideDoor?.Invoke();

        _shouldCheckOutsideDoor = true;
    }

    private void ResetRoom()
    {
        var monsterSM = MonsterStateMachine.Instance;
        monsterSM.GetMonsterState<CatchingPlayerMonsterState>().OnPlayerCatched -= ResetRoom;

        _openOutsideDoorTrigger.gameObject.SetActive(true);
        _tpMonsterThereTrigger.gameObject.SetActive(true);
        _closeOutsideDoorTrigger.gameObject.SetActive(true);

        _outsideDoorBlockade.gameObject.SetActive(false);
        transform.position = _lastRoomBasePos;

        _outsideRoomDoor.SetOpened(false);
        OnRoomReset?.Invoke();
    }
}

using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SingleSafeRoomHandler : MonoBehaviour
{
    public Action<SingleSafeRoomHandler> OnCheckpointReached;

    [Header("Checkpoint")]
    [SerializeField] private PlayerTargetTransform _checkpointPosition;
    [Header("Entry Door")]
    [SerializeField] private Door _entryDoor;
    [SerializeField] private Trigger _closeEntryDoorTrigger;
    [SerializeField] private GameObject _entryDoorBlockade;
    [Header("Exit Door")]
    [SerializeField] private Door _exitDoor;
    [SerializeField] private Trigger _closeExitDoorTrigger;
    [SerializeField] private GameObject _exitDoorBlockade;
    [Header("Monster")]
    [SerializeField] private bool _destroyMonster = false;
    [SerializeField] private MonsterStateMachine _stateMachine;  //can be null if killed
    [ShowIf(nameof(_destroyMonster)), SerializeField] private PerishingMonsterState _persihingState;
    [HideIf(nameof(_destroyMonster)), SerializeField] private Trigger _playerTpMonsterTrigger;
    [HideIf(nameof(_destroyMonster)), SerializeField] private Trigger _tpMonsterTrigger;
    [HideIf(nameof(_destroyMonster)), SerializeField] private TeleportingChosenMonsterState _tpChosenState;
    [HideIf(nameof(_destroyMonster)), SerializeField] private Transform _tpDirection;
    [HideIf(nameof(_destroyMonster)), SerializeField] private List<Transform> _newPatrolingPoints;

    private void Start()
    {
        _playerTpMonsterTrigger.OnObjectTriggerEnter += TeleportMonster;
        _closeEntryDoorTrigger.OnObjectTriggerEnter += HandleCheckpointReached;
        _closeExitDoorTrigger.OnObjectTriggerEnter += HandleCheckpointExit;
    }

    public void ResetToThisCheckpoint()
    {
        if (_checkpointPosition == null)
        {
            Debug.LogWarning("Reseted to checkpoint that has no position setuped!");
            return;
        }

        Debug.Log("Reseted to this checkpoint: " + gameObject.name);
        _tpMonsterTrigger.gameObject.SetActive(true);
        PlayerObjects.Instance.PlayerMovement.SetTransformInstant(_checkpointPosition, true);
        _stateMachine.EnableChangingStates();
        TeleportMonster();
        ResetExitDoor();
    }

    private void HandleCheckpointReached()
    {
        CloseDoor(_entryDoor, _closeEntryDoorTrigger, _entryDoorBlockade);
        Debug.Log("Checkpoint reached: " + gameObject.name);
        OnCheckpointReached?.Invoke(this);
    }

    private void HandleCheckpointExit()
    {
        CloseDoor(_exitDoor, _closeExitDoorTrigger, _exitDoorBlockade);
        TurnOffTpMonsterTrigger();
    }

    private void CloseDoor(Door door, Trigger trigger, GameObject blockade)
    {
        blockade.SetActive(true);
        trigger.gameObject.SetActive(false);

        //this is potentially bugged - if door in switch animation (closed -> opened) it wont work
        //closeDoorTrigger must be placed in the correct position
        //closing door must push player into it before end of animation - befor the could open it
        door.InteractionHitbox.gameObject.SetActive(false);
        if (door.IsOpened) door.SwitchDoorAnimated();
    }

    private void ResetExitDoor()
    {
        _exitDoorBlockade.SetActive(false);
        _closeExitDoorTrigger.gameObject.SetActive(true);
        _exitDoor.InteractionHitbox.gameObject.SetActive(true);
        _exitDoor.SetOpened(true);
    }

    private void TurnOffTpMonsterTrigger()
    {
        if (_stateMachine == null)
        {
            Debug.LogWarning("Monster is null. Was it killed?");
            return;
        }

        if (!_destroyMonster) _tpMonsterTrigger.gameObject.SetActive(false);
    }

    private void TeleportMonster()
    {
        _playerTpMonsterTrigger.gameObject.SetActive(false);

        if (_stateMachine == null)
        {
            Debug.LogWarning("Monster is null. Was it killed?");
            return;
        }

        if (_destroyMonster)
        {
            _stateMachine.ChangeState(_persihingState);
        }
        else
        {
            _stateMachine.ChangePatrolingPoints(_newPatrolingPoints);
            _tpChosenState.SetUpDestination(_tpDirection.position, true);
            _stateMachine.ChangeState(_tpChosenState);
        }
    }
}
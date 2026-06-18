using System.Collections.Generic;
using UnityEngine;

public class LastSafeRoom : Checkpoint
{
    [Header("Entry Door")]
    [SerializeField] private Door _entryDoor;
    [SerializeField] private Trigger _closeEntryDoorTrigger;
    [SerializeField] private GameObject _entryDoorBlockade;
    [Header("Exit Door")]
    [SerializeField] private Door _exitDoor;
    [SerializeField] private Trigger _closeExitDoorTrigger;
    [SerializeField] private GameObject _exitDoorBlockade;
    [Header("Monster")]
    [SerializeField] private MonsterStateMachine _stateMachine;  //can be null if killed
    [SerializeField] private Trigger _playerTpMonsterTrigger;
    [SerializeField] private TeleportingChosenMonsterState _tpChosenState;
    [SerializeField] private Transform _tpDirection;
    [SerializeField] private List<Transform> _newPatrolingPoints;

    private void Start()
    {
        _playerTpMonsterTrigger.OnObjectTriggerEnter += TeleportMonster;

        _closeEntryDoorTrigger.OnObjectTriggerEnter += () => CloseDoor(_entryDoor, _closeEntryDoorTrigger, _entryDoorBlockade);
        _closeEntryDoorTrigger.OnObjectTriggerEnter += InvokeCheckpointReached;
        _closeExitDoorTrigger.OnObjectTriggerEnter += () => CloseDoor(_exitDoor, _closeExitDoorTrigger, _exitDoorBlockade);
    }

    public override void ResetToThisCheckpoint()
    {
        if (_checkpointPosition == null)
        {
            Debug.LogWarning("Reseted to checkpoint that has no position setuped!");
            return;
        }

        Debug.Log("Reseted to this checkpoint: " + gameObject.name);
        PlayerObjects.Instance.PlayerMovement.SetTransformInstant(_checkpointPosition, true);
        TeleportMonster();
        ResetExitDoor();
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

    private void TeleportMonster()
    {
        _playerTpMonsterTrigger.gameObject.SetActive(false);

        if (_stateMachine == null)
        {
            Debug.LogWarning("Monster is null. Was it killed?");
            return;
        }

        _stateMachine.ChangePatrolingPoints(_newPatrolingPoints);
        _tpChosenState.SetUpDestination(_tpDirection.position, true);
        _stateMachine.ChangeState(_tpChosenState);
    }
}
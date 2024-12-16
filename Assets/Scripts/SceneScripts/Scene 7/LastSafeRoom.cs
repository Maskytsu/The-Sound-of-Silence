using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.LightAnchor;

public class LastSafeRoom : MonoBehaviour
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

        _closeEntryDoorTrigger.OnObjectTriggerEnter += () =>
            CloseDoor(_entryDoor, _closeEntryDoorTrigger, _entryDoorBlockade);

        _closeExitDoorTrigger.OnObjectTriggerEnter += () =>
            CloseDoor(_exitDoor, _closeExitDoorTrigger, _exitDoorBlockade);
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

    private void TeleportMonster()
    {
        _playerTpMonsterTrigger.gameObject.SetActive(false);

        if (_stateMachine == null)
        {
            Debug.LogWarning("Monster is null. Was it killed?");
            return;
        }

        _stateMachine.ChangePatrolingPoints(_newPatrolingPoints);
        _tpChosenState.SetUpDestination(_tpDirection.position);
        _stateMachine.ChangeState(_tpChosenState);
    }
}
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class SingleSafeRoomHandler : MonoBehaviour
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

        _closeEntryDoorTrigger.OnObjectTriggerEnter += () => 
            CloseDoor(_entryDoor, _closeEntryDoorTrigger, _entryDoorBlockade);

        _closeExitDoorTrigger.OnObjectTriggerEnter += () => 
            CloseDoor(_exitDoor, _closeExitDoorTrigger, _exitDoorBlockade);

        _closeExitDoorTrigger.OnObjectTriggerEnter += TurnOffTpMonsterTrigger;
    }

    private void CloseDoor(Door door, Trigger trigger, GameObject blockade)
    {
        blockade.SetActive(true);
        trigger.gameObject.SetActive(false);
        //this is bugged, if in switch animation (closed -> opened) it wont work
        door.InteractionHitbox.gameObject.SetActive(false);
        if (door.IsOpened) door.SwitchDoorAnimated();
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
            _tpChosenState.SetUpDestination(_tpDirection.position);
            _stateMachine.ChangeState(_tpChosenState);
        }
    }
}
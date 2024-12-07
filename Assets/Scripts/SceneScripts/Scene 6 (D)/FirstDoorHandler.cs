using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstDoorHandler : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _hiddingTutorial;
    [Header("Scene Objects")]
    [SerializeField] private Door _door;
    [SerializeField] private GameObject _doorBlockade;
    [SerializeField] private MonsterStateMachine _monsterStateMachine;
    [SerializeField] private WalkingChosenMonsterState _walkingChosenState;
    [SerializeField] private PlayerTrigger _closeDoorTrigger;
    [Header("Parameters")]
    [SerializeField] private int _startingPointIndex = 0;

    private GameObject _spawnedHiddingTutorial;

    private void Start()
    {
        _door.OnInteract += MoveMonster;

        _closeDoorTrigger.OnPlayerTriggerEnter += CloseDoor;
        _closeDoorTrigger.OnPlayerTriggerEnter += DisplayTutorial;
    }

    private void Update()
    {
        ManageDestroyingTutorial();
    }

    private void MoveMonster()
    {
        _door.OnInteract -= MoveMonster;
        _monsterStateMachine.gameObject.SetActive(true);
        _walkingChosenState.SetUpDestination(_startingPointIndex);
        _monsterStateMachine.ChangeState(_walkingChosenState);
    }

    private void CloseDoor()
    {
        _doorBlockade.SetActive(true);
        _closeDoorTrigger.gameObject.SetActive(false);
        _door.InteractionHitbox.gameObject.SetActive(false);
        if (_door.IsOpened) _door.SwitchDoorAnimated();
    }

    private void DisplayTutorial()
    {
        _spawnedHiddingTutorial = Instantiate(_hiddingTutorial);
    }

    private void ManageDestroyingTutorial()
    {
        if (_spawnedHiddingTutorial != null)
        {
            if (PlayerObjects.Instance.PlayerMovement.IsHidding)
            {
                Destroy(_spawnedHiddingTutorial.gameObject);
                _spawnedHiddingTutorial = null;
            }
        }
    }
}

using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstDoorHandler : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private TutorialOverlay _hideTutorial;
    [Header("Scene Objects")]
    [SerializeField] private Door _door;
    [SerializeField] private GameObject _doorBlockade;
    [SerializeField] private Trigger _closeDoorTrigger;
    [SerializeField] private Trigger _monsterTpTrigger;
    [Header("Parameters")]
    [SerializeField] private int _startingPointIndex = 0;

    private TutorialOverlay _spawnedHideTutorial;

    private bool _wasHidden;
    private bool _dashed;

    private void Start()
    {
        _door.OnInteract += MoveMonster;

        _closeDoorTrigger.OnObjectTriggerEnter += CloseDoor;
        _closeDoorTrigger.OnObjectTriggerEnter += StartTutorial;
    }

    private void Update()
    {
        ManageDestroyingTutorial();
    }

    private void MoveMonster()
    {
        _door.OnInteract -= MoveMonster;
        var monsterSM = MonsterStateMachine.Instance;
        var walkingChosenState = monsterSM.GetMonsterState<WalkingChosenMonsterState>();
        walkingChosenState.SetUpDestination(_startingPointIndex);
        monsterSM.ChangeState(walkingChosenState);
    }

    private void CloseDoor()
    {
        _doorBlockade.SetActive(true);
        _closeDoorTrigger.gameObject.SetActive(false);
        _door.InteractionHitbox.gameObject.SetActive(false);

        //this is potentially bugged - if door in switch animation (closed -> opened) it wont work
        //closeDoorTrigger must be placed in the correct position
        //closing door must push player into it before end of animation - befor the could open it
        if (_door.IsOpened) _door.SwitchDoorAnimated();
        _monsterTpTrigger.gameObject.SetActive(false);
    }

    private void StartTutorial()
    {
        GameManager.Instance.ChangeAwareModeState(true);
        _spawnedHideTutorial = Instantiate(_hideTutorial);
    }

    private void ManageDestroyingTutorial()
    {
        if (!_wasHidden || !_dashed)
        {
            if (PlayerObjects.Instance.PlayerMovement.IsHidding) _wasHidden = true;
            if (PlayerObjects.Instance.PlayerMovement.IsDashing) _dashed = true;

            if (_wasHidden && _dashed)
            {
                _spawnedHideTutorial.EndTutorial();
                _spawnedHideTutorial = null;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TeleportingMonsterState : MonsterState
{
    private Transform _chosenPosition;
    private Vector3 _teleportDestination;

    private MonsterStateCoroutines StateCoroutines => _stateMachine.StateCoroutines;

    public TeleportingMonsterState(MonsterStateMachine stateMachine, Transform chosenPosition = null)
    {
        _stateMachine = stateMachine;
        _chosenPosition = chosenPosition;
    }

    //---------------------------------------------------------------------------------------------------
    #region Implementing abstract methods
    public override void EnterState()
    {
        StateCoroutines.OnCoroutineEnd += StartPatrolingPoint;

        SetTeleportDestination();
        StateCoroutines.StartStateCoroutine(StateCoroutines.Teleport(_teleportDestination));
    }

    public override void Update()
    {
    }

    public override void ExitState()
    {
        StateCoroutines.OnCoroutineEnd -= StartPatrolingPoint;
    }
    #endregion
    //---------------------------------------------------------------------------------------------------

    private void SetTeleportDestination()
    {
        if (_chosenPosition != null)
        {
            _teleportDestination = _chosenPosition.position;
        }
        else
        {
            _teleportDestination = _stateMachine.RandomDifferentPositionPoint();
        }

        _teleportDestination.y = _stateMachine.MonsterTransform.position.y;
    }

    private void StartPatrolingPoint()
    {
        _stateMachine.ChangeState(new PatrolingPointMonsterState(_stateMachine));
    }
}

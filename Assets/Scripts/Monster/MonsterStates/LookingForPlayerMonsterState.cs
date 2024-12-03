using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LookingForPlayerMonsterState : MonsterState
{
    private Vector3 _lastSeenPlayerPosition;

    private NavMeshAgent NavMeshAgent => _stateMachine.NavMeshAgent;

    public LookingForPlayerMonsterState(MonsterStateMachine stateMachine, Vector3 lastSeenPlayerPosition)
    {
        _stateMachine = stateMachine;
        _lastSeenPlayerPosition = lastSeenPlayerPosition;
    }

    //---------------------------------------------------------------------------------------------------
    #region Implementing abstract methods
    public override void EnterState()
    {
        _stateMachine.MonsterFOV.OnStartSeeingPlayer += StartChasingPlayer;
        NavMeshAgent.enabled = true;
        NavMeshAgent.isStopped = false;
        SetDestination();
    }

    public override void Update()
    {
        CheckIfPathEndWasReached();
    }

    public override void ExitState()
    {
        _stateMachine.MonsterFOV.OnStartSeeingPlayer -= StartChasingPlayer;
        NavMeshAgent.isStopped = true;
        NavMeshAgent.enabled = false;
    }
    #endregion
    //---------------------------------------------------------------------------------------------------

    private void StartChasingPlayer()
    {
        _stateMachine.ChangeState(new ChasingPlayerMonsterState(_stateMachine));
    }

    private void SetDestination()
    {
        NavMeshAgent.SetDestination(_lastSeenPlayerPosition);
        //make it something like further then the actual last seen position
        //idk how yet - sample navmeshpoint forward by some distance?
    }

    private void CheckIfPathEndWasReached()
    {
        if (NavMeshAgent.remainingDistance == 0)
        {
            _stateMachine.ChangeState(new PatrolingPointMonsterState(_stateMachine));
        }
    }
}
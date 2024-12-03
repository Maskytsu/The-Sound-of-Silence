using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChasingPlayerMonsterState : MonsterState
{
    private event Action _onPlayerCatch;

    private bool _playerCatched = false;

    private NavMeshAgent NavMeshAgent => _stateMachine.NavMeshAgent;

    public ChasingPlayerMonsterState(MonsterStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    //---------------------------------------------------------------------------------------------------
    #region Implementing abstract methods
    public override void EnterState()
    {
        _stateMachine.MonsterFOV.OnStopSeeingPlayer += StartLookingForPlayer;
        _onPlayerCatch += StartCatchingPlayer;
        NavMeshAgent.enabled = true;
        NavMeshAgent.isStopped = false;
    }

    public override void Update()
    {
        ChasePlayer();
        CheckIfPlayerInCatchRange();
    }

    public override void ExitState()
    {
        _stateMachine.MonsterFOV.OnStopSeeingPlayer -= StartLookingForPlayer;
        _onPlayerCatch -= StartCatchingPlayer;
        _stateMachine.NavMeshAgent.isStopped = true;
        NavMeshAgent.enabled = false;
    }
    #endregion
    //---------------------------------------------------------------------------------------------------

    private void StartLookingForPlayer()
    {
        Vector3 lastSeenPlayerPosition = _stateMachine.MonsterFOV.SeenPlayerObj.transform.position;
        _stateMachine.ChangeState(new LookingForPlayerMonsterState(_stateMachine, lastSeenPlayerPosition));
    }

    private void StartCatchingPlayer()
    {
        _stateMachine.ChangeState(new CatchingPlayerMonsterState(_stateMachine));
    }

    private void ChasePlayer()
    {
        _stateMachine.NavMeshAgent.SetDestination(_stateMachine.MonsterFOV.SeenPlayerObj.transform.position);
    }

    private void CheckIfPlayerInCatchRange()
    {
        Vector3 playerPosition = _stateMachine.MonsterFOV.SeenPlayerObj.transform.position;
        Vector3 monsterPosition = _stateMachine.transform.position;

        playerPosition.y = 0f;
        monsterPosition.y = 0f;

        
        if (Vector3.Distance(playerPosition, monsterPosition) < _stateMachine.MonsterFOV.CatchRange)
        {
            if (!_playerCatched)
            {
                _playerCatched = true;
                _onPlayerCatch?.Invoke();
            }
        }
    }
}
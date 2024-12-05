using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.AI;

public class ChasingPlayerMonsterState : MonsterState
{
    [SerializeField] private float _chasingSpeed;
    [HorizontalLine, Header("Next states")]
    [SerializeField] private LookingForPlayerMonsterState _lookingForPlayerState;
    [SerializeField] private CatchingPlayerMonsterState _catchingPlayerState;

    //---------------------------------------------------------------------------------------------------
    private MonsterFieldOfView MonsterFOV => _stateMachine.MonsterFOV;
    private NavMeshAgent Agent => _stateMachine.Agent;
    private Transform MonsterTransform => _stateMachine.MonsterTransform;
    //---------------------------------------------------------------------------------------------------
    #region Implementing abstract methods
    public override void EnterState()
    {
        MonsterFOV.OnStopSeeingPlayer += StartLookingForPlayer;

        Agent.speed = _chasingSpeed;
        Agent.enabled = true;
        Agent.isStopped = false;
    }

    public override void StateUpdate()
    {
        ChasePlayer();
        ChangeStateIfPlayerInCatchRange();
    }

    public override void ExitState()
    {
        MonsterFOV.OnStopSeeingPlayer -= StartLookingForPlayer;

        Agent.isStopped = true;
        Agent.enabled = false;
    }
    #endregion
    //---------------------------------------------------------------------------------------------------

    private void StartLookingForPlayer()
    {
        Vector3 lastSeenPlayerPos = MonsterFOV.SeenPlayerObj.transform.position;
        _lookingForPlayerState.LastSeenPlayerPos = lastSeenPlayerPos;
        _stateMachine.ChangeState(_lookingForPlayerState);
    }

    private void ChasePlayer()
    {
        Agent.SetDestination(MonsterFOV.SeenPlayerObj.transform.position);
    }

    private void ChangeStateIfPlayerInCatchRange()
    {
        Vector3 playerPosition = MonsterFOV.SeenPlayerObj.transform.position;
        Vector3 monsterPosition = MonsterTransform.position;

        playerPosition.y = 0f;
        monsterPosition.y = 0f;

        
        if (Vector3.Distance(playerPosition, monsterPosition) < MonsterFOV.CatchRange)
        {
            _stateMachine.ChangeState(_catchingPlayerState);
        }
    }
}
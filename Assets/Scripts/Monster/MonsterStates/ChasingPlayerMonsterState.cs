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

    private event Action _onPlayerCatch;
    private bool _playerCatched;

    //---------------------------------------------------------------------------------------------------
    private MonsterFieldOfView MonsterFOV => _stateMachine.MonsterFOV;
    private NavMeshAgent Agent => _stateMachine.Agent;
    private Transform MonsterTransform => _stateMachine.MonsterTransform;
    //---------------------------------------------------------------------------------------------------
    #region Implementing abstract methods
    public override void EnterState()
    {
        _playerCatched = false;

        MonsterFOV.OnStopSeeingPlayer += StartLookingForPlayer;
        _onPlayerCatch += StartCatchingPlayer;

        Agent.speed = _chasingSpeed;
        Agent.enabled = true;
        Agent.isStopped = false;
    }

    public override void StateUpdate()
    {
        ChasePlayer();
        CheckIfPlayerInCatchRange();
    }

    public override void ExitState()
    {
        MonsterFOV.OnStopSeeingPlayer -= StartLookingForPlayer;
        _onPlayerCatch -= StartCatchingPlayer;

        Agent.isStopped = true;
        Agent.enabled = false;
    }
    #endregion
    //---------------------------------------------------------------------------------------------------

    private void StartLookingForPlayer()
    {
        Vector3 lastSeenPlayerPosition = MonsterFOV.SeenPlayerObj.transform.position;
        _lookingForPlayerState.LastSeenPlayerPosition = lastSeenPlayerPosition;
        _stateMachine.ChangeState(_lookingForPlayerState);
    }

    private void StartCatchingPlayer()
    {
        _stateMachine.ChangeState(_catchingPlayerState);
    }

    private void ChasePlayer()
    {
        Agent.SetDestination(MonsterFOV.SeenPlayerObj.transform.position);
    }

    private void CheckIfPlayerInCatchRange()
    {
        Vector3 playerPosition = MonsterFOV.SeenPlayerObj.transform.position;
        Vector3 monsterPosition = MonsterTransform.position;

        playerPosition.y = 0f;
        monsterPosition.y = 0f;

        
        if (Vector3.Distance(playerPosition, monsterPosition) < MonsterFOV.CatchRange)
        {
            if (!_playerCatched)
            {
                _playerCatched = true;
                _onPlayerCatch?.Invoke();
            }
        }
    }
}
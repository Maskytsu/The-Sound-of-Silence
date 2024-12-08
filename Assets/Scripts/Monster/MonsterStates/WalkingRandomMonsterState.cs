using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

public class WalkingRandomMonsterState : MonsterState
{
    [SerializeField] private float _walkingSpeed;
    [HorizontalLine, Header("Next states")]
    [SerializeField] private PatrolingPointMonsterState _patrolingPointState;
    [SerializeField] private ChasingPlayerMonsterState _chasingPlayerState;

    //---------------------------------------------------------------------------------------------------
    private NavMeshAgent Agent => _stateMachine.Agent;
    //---------------------------------------------------------------------------------------------------
    #region Implementing abstract methods
    public override void EnterState()
    {
        _stateMachine.MonsterFOV.OnStartSeeingPlayer += StartChasingPlayer;

        Agent.speed = _walkingSpeed;
        Agent.enabled = true;
        Agent.isStopped = false;

        SetDestination();
    }

    public override void StateUpdate()
    {
        PatrolPointOnPathEnd();
    }

    public override void ExitState()
    {
        _stateMachine.MonsterFOV.OnStartSeeingPlayer -= StartChasingPlayer;

        Agent.isStopped = true;
        Agent.enabled = false;
    }
    #endregion
    //---------------------------------------------------------------------------------------------------

    private void StartChasingPlayer()
    {
        _stateMachine.ChangeState(_chasingPlayerState);
    }

    protected virtual void SetDestination() 
    {
        Agent.SetDestination(_stateMachine.RandomDifferentPositionPoint());
    }

    private void PatrolPointOnPathEnd()
    {
        if (!Agent.pathPending && Agent.remainingDistance == 0) 
        {
            _stateMachine.ChangeState(_patrolingPointState);
        }
    }
}
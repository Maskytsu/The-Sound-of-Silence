using UnityEngine;
using UnityEngine.AI;

public class WalkingMonsterState : MonsterState
{
    private Transform _chosenPosition;

    private NavMeshAgent NavMeshAgent => _stateMachine.NavMeshAgent;

    public WalkingMonsterState(MonsterStateMachine stateMachine, Transform chosenPosition = null)
    {
        _stateMachine = stateMachine;
        _chosenPosition = chosenPosition;
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
        if (_chosenPosition != null)
        {
            NavMeshAgent.SetDestination(_chosenPosition.position);
        }
        else
        {
            NavMeshAgent.SetDestination(_stateMachine.RandomDifferentPositionPoint());
        }
    }

    private void CheckIfPathEndWasReached()
    {
        if (NavMeshAgent.remainingDistance == 0)
        {
            _stateMachine.ChangeState(new PatrolingPointMonsterState(_stateMachine));
        }
    }
}

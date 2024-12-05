using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

public class LookingForPlayerMonsterState : MonsterState
{
    [HideInInspector] public Vector3? LastSeenPlayerPosition;

    [HorizontalLine, Header("Next states")]
    [SerializeField] private ChasingPlayerMonsterState _chasingPlayerState;
    [SerializeField] private PatrolingPointMonsterState _patrolingPointState;

    //---------------------------------------------------------------------------------------------------
    private MonsterFieldOfView MonsterFOV => _stateMachine.MonsterFOV;
    private NavMeshAgent Agent => _stateMachine.Agent;
    //---------------------------------------------------------------------------------------------------
    #region Implementing abstract methods
    public override void EnterState()
    {
        MonsterFOV.OnStartSeeingPlayer += StartChasingPlayer;

        Agent.enabled = true;
        Agent.isStopped = false;

        SetDestination();
    }

    public override void StateUpdate()
    {
        CheckIfPathEndWasReached();
    }

    public override void ExitState()
    {
        LastSeenPlayerPosition = null;

        MonsterFOV.OnStartSeeingPlayer -= StartChasingPlayer;

        Agent.isStopped = true;
        Agent.enabled = false;
    }
    #endregion
    //---------------------------------------------------------------------------------------------------

    private void StartChasingPlayer()
    {
        _stateMachine.ChangeState(_chasingPlayerState);
    }

    private void SetDestination()
    {
        Agent.SetDestination(LastSeenPlayerPosition.Value);
        //make it something like further then the actual last seen position
        //idk how yet - sample navmeshpoint forward by some distance?
    }

    private void CheckIfPathEndWasReached()
    {
        if (Agent.remainingDistance == 0)
        {
            _stateMachine.ChangeState(_patrolingPointState);
        }
    }
}
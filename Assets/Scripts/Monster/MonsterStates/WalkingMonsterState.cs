using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

public class WalkingMonsterState : MonsterState
{
    [HideInInspector] public Vector3? ChosenPosition;

    [SerializeField] private float _walkingSpeed;
    [HorizontalLine, Header("Next states")]
    [SerializeField] private PatrolingPointMonsterState _patrolingPointState;
    [SerializeField] private ChasingPlayerMonsterState _chasingPlayerState;

    //---------------------------------------------------------------------------------------------------
    private MonsterFieldOfView MonsterFOV => _stateMachine.MonsterFOV;
    private NavMeshAgent Agent => _stateMachine.Agent;
    //---------------------------------------------------------------------------------------------------
    #region Implementing abstract methods
    public override void EnterState()
    {
        MonsterFOV.OnStartSeeingPlayer += StartChasingPlayer;

        Agent.speed = _walkingSpeed;
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
        ChosenPosition = null;

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
        if (ChosenPosition != null) Agent.SetDestination(ChosenPosition.Value);
        else Agent.SetDestination(_stateMachine.RandomDifferentPositionPoint());
    }

    private void CheckIfPathEndWasReached()
    {
        if (Agent.remainingDistance == 0) 
        {
            _stateMachine.ChangeState(_patrolingPointState);
        }
    }
}

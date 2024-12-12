using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class WalkingRandomMonsterState : MonsterState
{
    [SerializeField] private float _walkingSpeed;
    [HorizontalLine, Header("Next states")]
    [SerializeField] private PatrolingPointMonsterState _patrolingPointState;
    [SerializeField] private ChasingPlayerMonsterState _chasingPlayerState;
    [SerializeField] private TeleportingChosenMonsterState _telportingChosenState;

    protected Vector3? _chosenDestination;

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
        _chosenDestination = null;

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
        _chosenDestination = _stateMachine.RandomDifferentPositionPoint();
        Agent.SetDestination(_chosenDestination.Value);
    }

    private void PatrolPointOnPathEnd()
    {
        if (!Agent.pathPending && Agent.remainingDistance == 0) 
        {
            float distance = Vector3.Distance(Agent.pathEndPosition, _chosenDestination.Value);

            if (distance > 3)
            {
                _telportingChosenState.SetUpDestination(_chosenDestination.Value);
                _stateMachine.ChangeState(_telportingChosenState);
            }
            else
            {
                _stateMachine.ChangeState(_patrolingPointState);
            }
        }
    }
}
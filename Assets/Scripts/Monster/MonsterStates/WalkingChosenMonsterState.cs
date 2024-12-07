using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

public class WalkingChosenMonsterState : MonsterState
{
    [SerializeField] private float _walkingSpeed;
    [HorizontalLine, Header("Next states")]
    [SerializeField] private PatrolingPointMonsterState _patrolingPointState;
    [SerializeField] private ChasingPlayerMonsterState _chasingPlayerState;

    private Vector3? _chosenDestination;

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

    public void SetUpDestination(Vector3 position)
    {
        _chosenDestination = position;
    }

    public void SetUpDestination(int pointsListIndex)
    {
        _chosenDestination = _stateMachine.GetPositionFromPointsList(pointsListIndex);
    }

    private void StartChasingPlayer()
    {
        _stateMachine.ChangeState(_chasingPlayerState);
    }

    private void SetDestination()
    {
        if (_chosenDestination == null)
        {
            Debug.LogError("Destination wasn't set up befor changing to this state!");
            _chosenDestination = _stateMachine.MonsterTransform.position;
        }

        Agent.SetDestination(_chosenDestination.Value);
    }

    private void PatrolPointOnPathEnd()
    {
        if (Agent.remainingDistance == 0)
        {
            _stateMachine.ChangeState(_patrolingPointState);
        }
    }
}
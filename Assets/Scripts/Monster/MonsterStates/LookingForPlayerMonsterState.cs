using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

public class LookingForPlayerMonsterState : MonsterState
{
    [SerializeField] private float _lookingForSpeed;
    [SerializeField] private float _lookingForDistance;
    [SerializeField] private LayerMask _obstacleMask;
    [HorizontalLine, Header("Next states")]
    [SerializeField] private ChasingPlayerMonsterState _chasingPlayerState;
    [SerializeField] private PatrolingPointMonsterState _patrolingPointState;

    private bool _pathExtended;

    //---------------------------------------------------------------------------------------------------
    private NavMeshAgent Agent => _stateMachine.Agent;
    private Transform MonsterTransform => _stateMachine.MonsterTransform;
    //---------------------------------------------------------------------------------------------------
    #region Implementing abstract methods
    public override void EnterState()
    {
        _pathExtended = false;

        _stateMachine.MonsterFOV.OnStartSeeingPlayer += StartChasingPlayer;

        Agent.enabled = true;
        Agent.isStopped = false;

        SetLastSeenPositionAsDestination();
    }

    public override void StateUpdate()
    {
        ExtendPathOnFirstNearPathEnd();
        PatrolPointOnPathEnd();
    }

    public override void ExitState()
    {
        _stateMachine.MonsterFOV.OnStartSeeingPlayer -= StartChasingPlayer;

        Agent.speed = _lookingForSpeed;
        Agent.isStopped = true;
        Agent.enabled = false;
    }
    #endregion
    //---------------------------------------------------------------------------------------------------

    private void StartChasingPlayer()
    {
        _stateMachine.ChangeState(_chasingPlayerState);
    }

    private void SetLastSeenPositionAsDestination()
    {
        Vector3 lastSeenPlayerPos = PlayerObjects.Instance.Player.transform.position;
        Agent.SetDestination(lastSeenPlayerPos);
    }

    private void ExtendPathOnFirstNearPathEnd()
    {
        if (!_pathExtended && Agent.remainingDistance < 0.5f)
        {
            _pathExtended = true;

            while (true)
            {
                if (Physics.Raycast(MonsterTransform.position, MonsterTransform.forward, _lookingForDistance, _obstacleMask))
                {
                    _lookingForDistance -= 0.25f;
                }
                else
                {
                    break;
                }
            }

            Vector3 extendedPos = MonsterTransform.position + (MonsterTransform.forward * _lookingForDistance);
            Agent.SetDestination(extendedPos);
        }
    }

    private void PatrolPointOnPathEnd()
    {
        if (!Agent.pathPending && Agent.remainingDistance == 0)
        {
            _stateMachine.ChangeState(_patrolingPointState);
        }
    }
}
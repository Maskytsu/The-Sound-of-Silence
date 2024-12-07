using FMODUnity;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class ChasingPlayerMonsterState : MonsterState
{
    [SerializeField] private EventReference _sawPlayerSound;
    [SerializeField] private float _chasingSpeed;
    [Tooltip("It should be bigger than the one on state machine")]
    [SerializeField] private float _biggerCatchingRange;
    [HorizontalLine, Header("Next states")]
    [SerializeField] private LookingForPlayerMonsterState _lookingForPlayerState;
    [SerializeField] private CatchingPlayerMonsterState _catchingPlayerState;

    //---------------------------------------------------------------------------------------------------
    private MonsterFieldOfView MonsterFOV => _stateMachine.MonsterFOV;
    private NavMeshAgent Agent => _stateMachine.Agent;
    //---------------------------------------------------------------------------------------------------
    #region Implementing abstract methods
    public override void EnterState()
    {
        AudioManager.Instance.PlayOneShotOccluded(_sawPlayerSound, _stateMachine.MonsterTransform);

        MonsterFOV.OnStopSeeingPlayer += StartLookingForPlayer;

        Agent.speed = _chasingSpeed;
        Agent.enabled = true;
        Agent.isStopped = false;
    }

    public override void StateUpdate()
    {
        ChasePlayer();
        CatchPlayerIfInCatchRange();
    }

    public override void ExitState()
    {
        MonsterFOV.OnStopSeeingPlayer -= StartLookingForPlayer;

        Agent.isStopped = true;
        Agent.enabled = false;
    }
    #endregion
    //---------------------------------------------------------------------------------------------------

    private void OnDrawGizmos()
    {
        DrawCatchRange();
    }

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

    private void CatchPlayerIfInCatchRange()
    {
        Vector3 playerPosition = MonsterFOV.SeenPlayerObj.transform.position;
        Vector3 monsterPosition = _stateMachine.MonsterTransform.position;

        playerPosition.y = 0f;
        monsterPosition.y = 0f;

        
        if (Vector3.Distance(playerPosition, monsterPosition) < _biggerCatchingRange)
        {
            _stateMachine.ChangeState(_catchingPlayerState);
        }
    }

    private void DrawCatchRange()
    {
        Handles.color = Color.red;
        Handles.DrawWireArc(MonsterFOV.FOVStartingPoint.position, Vector3.up, Vector3.forward, 360, _biggerCatchingRange);
    }
}
using FMODUnity;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class OnHitChasingPlayerMonsterState : MonsterState
{
    [SerializeField] protected float _chasingSpeed;
    [Tooltip("It should be bigger than the one on state machine")]
    [SerializeField] private float _biggerCatchingRange;
    [HorizontalLine, Header("Next states")]
    [SerializeField] private CatchingPlayerMonsterState _catchingPlayerState;

    //---------------------------------------------------------------------------------------------------
    private Vector3 PlayerPos => PlayerObjects.Instance.Player.transform.position;
    protected NavMeshAgent Agent => _stateMachine.Agent;
    //---------------------------------------------------------------------------------------------------
    #region Implementing abstract methods
    public override void EnterState()
    {
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
        Agent.isStopped = true;
        Agent.enabled = false;
    }
    #endregion
    //---------------------------------------------------------------------------------------------------

    private void OnDrawGizmos()
    {
        DrawCatchRange();
    }

    protected void ChasePlayer()
    {
        Agent.SetDestination(PlayerPos);
    }

    protected void CatchPlayerIfInCatchRange()
    {
        Vector3 playerPosition = PlayerPos;
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
        #if UNITY_EDITOR
        Handles.color = Color.red;
        Vector3 fovPoint = _stateMachine.MonsterFOV.FOVStartingPoint.position;
        Handles.DrawWireArc(fovPoint, Vector3.up, Vector3.forward, 360, _biggerCatchingRange);
        #endif
    }
}
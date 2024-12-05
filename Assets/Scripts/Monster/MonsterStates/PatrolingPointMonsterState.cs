using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.AI;

public class PatrolingPointMonsterState : MonsterState
{
    [HorizontalLine, Header("Next states")]
    [SerializeField] private WalkingMonsterState _walkingState;
    [SerializeField] private TeleportingMonsterState _teleportingState;
    [SerializeField] private ChasingPlayerMonsterState _chasingPlayerState;

    private event Action _onPatrolEnd;
    private float _currentRotationAngle;

    private float _rotationTime = 4f;

    //---------------------------------------------------------------------------------------------------
    private MonsterFieldOfView MonsterFOV => _stateMachine.MonsterFOV;
    private Transform MonsterTransform => _stateMachine.MonsterTransform;
    //---------------------------------------------------------------------------------------------------
    #region Implementing abstract methods
    public override void EnterState()
    {
        _currentRotationAngle = 0f;

        MonsterFOV.OnStartSeeingPlayer += StartChasingPlayer;
        _onPatrolEnd += WalkOrTeleportRandomized;
    }

    public override void StateUpdate()
    {
        PatrolPoint();
    }

    public override void ExitState()
    {
        MonsterFOV.OnStartSeeingPlayer -= StartChasingPlayer;
        _onPatrolEnd -= WalkOrTeleportRandomized;
    }
    #endregion
    //---------------------------------------------------------------------------------------------------

    private void StartChasingPlayer()
    {
        _stateMachine.ChangeState(_chasingPlayerState);
    }

    private void PatrolPoint()
    {
        if (_currentRotationAngle >= 355f) return;

        float rotationAmount = (360 * Time.deltaTime) / _rotationTime;
        MonsterTransform.Rotate(new Vector3(0, rotationAmount, 0));

        _currentRotationAngle += rotationAmount;
        if (_currentRotationAngle >= 355f) _onPatrolEnd?.Invoke();
    }

    private void WalkOrTeleportRandomized()
    {
        //chance 1/3 for random to be 0
        int random = UnityEngine.Random.Range(0, 3);

        if (random == 0) _stateMachine.ChangeState(_teleportingState);
        else _stateMachine.ChangeState(_walkingState);
    }
}
using System;
using UnityEngine;

public class PatrolingPointMonsterState : MonsterState
{
    private event Action _onPatrolEnd;

    private float _rotationTime = 3f;
    private float _currentRotationAngle = 0f;

    public PatrolingPointMonsterState(MonsterStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    //---------------------------------------------------------------------------------------------------
    #region Implementing abstract methods
    public override void EnterState()
    {
        _stateMachine.MonsterFOV.OnStartSeeingPlayer += StartChasingPlayer;
        _onPatrolEnd += WalkOrTeleportRandomized;
    }

    public override void Update()
    {
        PatrolPoint();
    }

    public override void ExitState()
    {
        _stateMachine.MonsterFOV.OnStartSeeingPlayer -= StartChasingPlayer;
        _onPatrolEnd -= WalkOrTeleportRandomized;
    }
    #endregion
    //---------------------------------------------------------------------------------------------------

    private void StartChasingPlayer()
    {
        _stateMachine.ChangeState(new ChasingPlayerMonsterState(_stateMachine));
    }

    private void PatrolPoint()
    {
        if (_currentRotationAngle >= 355f) return;

        float rotationAmount = (360 * Time.deltaTime) / _rotationTime;
        _stateMachine.MonsterTransform.Rotate(new Vector3(0, rotationAmount, 0));

        _currentRotationAngle += rotationAmount;
        if (_currentRotationAngle >= 355f) _onPatrolEnd?.Invoke();
    }

    private void WalkOrTeleportRandomized()
    {
        //chance 2/5 for random to be 0 or 1
        int random = UnityEngine.Random.Range(0, 5);

        if (random == 0 || random == 1) _stateMachine.ChangeState(new TeleportingMonsterState(_stateMachine));
        else _stateMachine.ChangeState(new WalkingMonsterState(_stateMachine));
    }
}
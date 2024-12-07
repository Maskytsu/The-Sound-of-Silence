using NaughtyAttributes;
using UnityEngine;

public class PatrolingPointMonsterState : MonsterState
{
    [SerializeField] private float _rotationTime;
    [HorizontalLine, Header("Next states")]
    [SerializeField] private WalkingRandomMonsterState _walkingState;
    [SerializeField] private TeleportingRandomMonsterState _teleportingState;
    [SerializeField] private ChasingPlayerMonsterState _chasingPlayerState;

    private float _currentRotationAngle;

    //---------------------------------------------------------------------------------------------------
    #region Implementing abstract methods
    public override void EnterState()
    {
        _currentRotationAngle = 0f;

        _stateMachine.MonsterFOV.OnStartSeeingPlayer += StartChasingPlayer;
    }

    public override void StateUpdate()
    {
        PatrolPoint();
    }

    public override void ExitState()
    {
        _stateMachine.MonsterFOV.OnStartSeeingPlayer -= StartChasingPlayer;
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
        _stateMachine.MonsterTransform.Rotate(new Vector3(0, rotationAmount, 0));

        _currentRotationAngle += rotationAmount;
        if (_currentRotationAngle >= 355f) ChangeToWalkOrTeleportRandomized();
    }

    private void ChangeToWalkOrTeleportRandomized()
    {
        //chance 1/3 for random to be 0
        int random = UnityEngine.Random.Range(0, 3);

        if (random == 0) _stateMachine.ChangeState(_teleportingState);
        else _stateMachine.ChangeState(_walkingState);
    }
}
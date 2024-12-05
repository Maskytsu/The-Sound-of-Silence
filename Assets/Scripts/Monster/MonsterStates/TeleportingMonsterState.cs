using FMODUnity;
using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;

public class TeleportingMonsterState : MonsterState
{
    [HideInInspector] public Vector3? ChosenPosition;

    [SerializeField] private EventReference _monsterTPSound;
    [HorizontalLine, Header("Next states")]
    [SerializeField] private PatrolingPointMonsterState _patrolingPointState;

    private event Action _onTeleportingEnd;

    //---------------------------------------------------------------------------------------------------
    private Transform MonsterTransform => _stateMachine.MonsterTransform;
    //---------------------------------------------------------------------------------------------------
    #region Implementing abstract methods
    public override void EnterState()
    {
        _onTeleportingEnd += StartPatrolingPoint;

        Vector3 tpDestination = ChooseTeleportDestination();
        StartCoroutine(Teleport(tpDestination));
    }

    public override void StateUpdate()
    {
    }

    public override void ExitState()
    {
        ChosenPosition = null;
        StopAllCoroutines();

        _onTeleportingEnd -= StartPatrolingPoint;
    }
    #endregion
    //---------------------------------------------------------------------------------------------------

    private void StartPatrolingPoint()
    {
        _stateMachine.ChangeState(_patrolingPointState);
    }

    private Vector3 ChooseTeleportDestination()
    {
        Vector3 tpDestination;

        if (ChosenPosition != null) tpDestination = ChosenPosition.Value;
        else tpDestination = _stateMachine.RandomDifferentPositionPoint();

        tpDestination.y = MonsterTransform.position.y;

        return tpDestination;
    }

    public IEnumerator Teleport(Vector3 tpDestination)
    {
        AudioManager.Instance.PlayOneShotOccluded(_monsterTPSound, MonsterTransform);
        yield return new WaitForSeconds(1.5f);
        MonsterTransform.position = tpDestination;
        AudioManager.Instance.PlayOneShotOccluded(_monsterTPSound, MonsterTransform);
        yield return new WaitForSeconds(1.5f);

        _onTeleportingEnd?.Invoke();
    }
}

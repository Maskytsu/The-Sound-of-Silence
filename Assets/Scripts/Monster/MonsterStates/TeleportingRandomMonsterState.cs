using FMODUnity;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class TeleportingRandomMonsterState : MonsterState
{
    [SerializeField] private EventReference _monsterTPSound;
    [HorizontalLine, Header("Next states")]
    [SerializeField] private PatrolingPointMonsterState _patrolingPointState;

    //---------------------------------------------------------------------------------------------------
    private Transform MonsterTransform => _stateMachine.MonsterTransform;
    //---------------------------------------------------------------------------------------------------
    #region Implementing abstract methods
    public override void EnterState()
    {
        StartCoroutine(Teleport());
    }

    public override void StateUpdate()
    {
    }

    public override void ExitState()
    {
        StopAllCoroutines();
    }
    #endregion
    //---------------------------------------------------------------------------------------------------

    public IEnumerator Teleport()
    {
        AudioManager.Instance.PlayOneShotOccluded(_monsterTPSound, MonsterTransform);
        yield return new WaitForSeconds(1.5f);
        MonsterTransform.position = TeleportDestination();
        AudioManager.Instance.PlayOneShotOccluded(_monsterTPSound, MonsterTransform);
        yield return new WaitForSeconds(1.5f);

        _stateMachine.ChangeState(_patrolingPointState);
    }

    private Vector3 TeleportDestination()
    {
        Vector3 tpDestination = _stateMachine.RandomDifferentPositionPoint();
        tpDestination.y = MonsterTransform.position.y;

        return tpDestination;
    }
}

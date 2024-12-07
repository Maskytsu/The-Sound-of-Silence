using FMODUnity;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class TeleportingChosenMonsterState : MonsterState
{
    [SerializeField] private EventReference _monsterTPSound;
    [HorizontalLine, Header("Next states")]
    [SerializeField] private PatrolingPointMonsterState _patrolingPointState;

    public Vector3? _chosenDestination;

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
        _chosenDestination = null;

        StopAllCoroutines();
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

    private IEnumerator Teleport()
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
        if (_chosenDestination == null)
        {
            Debug.LogError("Destination wasn't set up befor changing to this state!");
            return _stateMachine.MonsterTransform.position;
        }

        Vector3 tpDestination = _chosenDestination.Value;
        tpDestination.y = MonsterTransform.position.y;

        return tpDestination;
    }
}

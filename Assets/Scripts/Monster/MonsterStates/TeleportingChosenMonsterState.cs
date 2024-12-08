using UnityEngine;

public class TeleportingChosenMonsterState : TeleportingRandomMonsterState
{
    private Vector3? _chosenDestination;

    public void SetUpDestination(Vector3 position)
    {
        _chosenDestination = position;
    }

    public void SetUpDestination(int pointsListIndex)
    {
        _chosenDestination = _stateMachine.GetPositionFromPointsList(pointsListIndex);
    }

    protected override Vector3 TeleportDestination()
    {
        if (_chosenDestination == null)
        {
            Debug.LogError("Destination wasn't set up befor changing to this state!");
            return _stateMachine.MonsterTransform.position;
        }

        Vector3 tpDestination = _chosenDestination.Value;
        tpDestination.y = _stateMachine.MonsterTransform.position.y;

        return tpDestination;
    }
}
using UnityEngine;

public class TeleportingChosenMonsterState : TeleportingRandomMonsterState
{
    private Vector3 _chosenDestination;

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
        Vector3 tpDestination = _chosenDestination;
        tpDestination.y = _stateMachine.MonsterTransform.position.y;

        return tpDestination;
    }
}
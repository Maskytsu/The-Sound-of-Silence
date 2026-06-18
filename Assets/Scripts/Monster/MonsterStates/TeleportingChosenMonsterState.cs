using UnityEngine;

public class TeleportingChosenMonsterState : TeleportingRandomMonsterState
{
    private Vector3 _chosenDestination;
    private bool _isInstant = false;

    protected override bool IsInstant => _isInstant;

    public void SetUpDestination(Vector3 position, bool isInstant = false)
    {
        _chosenDestination = position;
        _isInstant = isInstant;
    }

    public void SetUpDestination(int pointsListIndex, bool isInstant = false)
    {
        _chosenDestination = _stateMachine.GetPositionFromPointsList(pointsListIndex);
        _isInstant = isInstant;
    }

    protected override Vector3 TeleportDestination()
    {
        Vector3 tpDestination = _chosenDestination;
        tpDestination.y = _stateMachine.MonsterTransform.position.y;

        return tpDestination;
    }
}
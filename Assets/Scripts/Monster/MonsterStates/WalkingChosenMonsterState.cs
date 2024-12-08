using UnityEngine;

public class WalkingChosenMonsterState : WalkingRandomMonsterState
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

    protected override void SetDestination()
    {
        if (_chosenDestination == null)
        {
            Debug.LogError("Destination wasn't set up befor changing to this state!");
            _chosenDestination = _stateMachine.MonsterTransform.position;
        }

        _stateMachine.Agent.SetDestination(_chosenDestination.Value);
    }
}
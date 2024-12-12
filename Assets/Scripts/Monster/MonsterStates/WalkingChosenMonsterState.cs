using Unity.VisualScripting;
using UnityEngine;

public class WalkingChosenMonsterState : WalkingRandomMonsterState
{
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
        _stateMachine.Agent.SetDestination(_chosenDestination.Value);
    }
}
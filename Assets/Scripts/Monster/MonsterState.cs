using UnityEngine;

public abstract class MonsterState : MonoBehaviour
{
    [SerializeField] protected MonsterStateMachine _stateMachine;

    public abstract void EnterState();
    public abstract void StateUpdate();
    public abstract void ExitState();
}
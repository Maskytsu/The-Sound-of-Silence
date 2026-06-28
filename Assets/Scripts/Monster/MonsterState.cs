using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public abstract class MonsterState : MonoBehaviour
{
    [SerializeField] private UnityEvent OnEnterStateUE = new();
    [SerializeField] private UnityEvent OnExitStateUE = new();
    [Space]
    [SerializeField] protected MonsterStateMachine _stateMachine;

    public abstract void EnterState();
    public abstract void StateUpdate();
    public abstract void ExitState();

    public void InvokeOnEnter()
    {
        OnEnterStateUE?.Invoke();
    }

    public void InvokeOnExit()
    {
        OnExitStateUE?.Invoke();
    }

    [Button]
    protected void ChangeToThisState()
    {
        _stateMachine.ChangeState(this);
    }
}
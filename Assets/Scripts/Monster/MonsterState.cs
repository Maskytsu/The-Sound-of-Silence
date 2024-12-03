using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterState
{
    protected MonsterStateMachine _stateMachine;

    public abstract void EnterState();
    public abstract void Update();
    public abstract void ExitState();
}
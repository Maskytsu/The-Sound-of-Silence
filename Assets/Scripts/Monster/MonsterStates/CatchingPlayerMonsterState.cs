using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CatchingPlayerMonsterState : MonsterState
{
    private MonsterStateCoroutines StateCoroutines => _stateMachine.StateCoroutines;

    public CatchingPlayerMonsterState(MonsterStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    //---------------------------------------------------------------------------------------------------
    #region Implementing abstract methods
    public override void EnterState()
    {
        StateCoroutines.StartStateCoroutine(StateCoroutines.CatchingAnimation());
    }

    public override void Update()
    {
    }

    public override void ExitState()
    {
    }
    #endregion
    //---------------------------------------------------------------------------------------------------
}
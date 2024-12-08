using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerishingMonsterState : MonsterState
{
    //---------------------------------------------------------------------------------------------------
    #region Implementing abstract methods
    public override void EnterState()
    {
        Debug.LogWarning("Perishing state is not implemented yet!");
        Destroy(_stateMachine.MonsterTransform.gameObject);
    }

    public override void StateUpdate()
    {
    }

    public override void ExitState()
    {
    }
    #endregion
    //---------------------------------------------------------------------------------------------------
}

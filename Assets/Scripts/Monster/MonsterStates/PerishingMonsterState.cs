using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerishingMonsterState : MonsterState
{
    [SerializeField] private Light _lightCone;
    [SerializeField] private MeshRenderer _eyeMesh;
    [SerializeField] private Material _eyePerishMaterial;
    [SerializeField] private Color _perishLightColor;

    //---------------------------------------------------------------------------------------------------
    #region Implementing abstract methods
    public override void EnterState()
    {
        _stateMachine.DisableChangingStates();

        StartCoroutine(Perish());
    }

    public override void StateUpdate()
    {
    }

    public override void ExitState()
    {
    }
    #endregion
    //---------------------------------------------------------------------------------------------------

    private IEnumerator Perish()
    {
        _eyeMesh.material = _eyePerishMaterial;
        _lightCone.color = _perishLightColor;

        AudioManager.Instance.PlayOneShotOccludedRI(FmodEvents.Instance.OCC_MonsterPerish, _stateMachine.MonsterTransform);

        yield return new WaitForSeconds(2f);
        Destroy(_stateMachine.MonsterTransform.gameObject);
    }
}

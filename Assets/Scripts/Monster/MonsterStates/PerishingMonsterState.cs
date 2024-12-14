using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerishingMonsterState : MonsterState
{
    [SerializeField] private EventReference _monsterPerishSound;
    [SerializeField] private Light _lightCone;
    [SerializeField] private MeshRenderer _eyeMesh;
    [SerializeField] private MeshRenderer _headMesh;
    [SerializeField] private Material _eyeTpMaterial;
    [SerializeField] private Material _headTpMaterial;

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
        _eyeMesh.material = _eyeTpMaterial;
        _headMesh.material = _headTpMaterial;
        _lightCone.enabled = false;

        AudioManager.Instance.PlayOneShotOccluded(_monsterPerishSound, _stateMachine.MonsterTransform);

        yield return new WaitForSeconds(2f);
        Destroy(_stateMachine.MonsterTransform.gameObject);
    }
}

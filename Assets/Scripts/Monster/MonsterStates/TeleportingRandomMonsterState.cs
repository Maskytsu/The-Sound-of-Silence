using FMODUnity;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class TeleportingRandomMonsterState : MonsterState
{
    [SerializeField] private EventReference _monsterTPSound;
    [SerializeField] private MeshRenderer _eyeMesh;
    [SerializeField] private MeshRenderer _headMesh;
    [SerializeField] private Material _eyeTpMaterial;
    [SerializeField] private Material _headTpMaterial;
    [HorizontalLine, Header("Next states")]
    [SerializeField] private PatrolingPointMonsterState _patrolingPointState;

    //---------------------------------------------------------------------------------------------------
    private Transform MonsterTransform => _stateMachine.MonsterTransform;
    //---------------------------------------------------------------------------------------------------
    #region Implementing abstract methods
    public override void EnterState()
    {
        StartCoroutine(Teleport());
    }

    public override void StateUpdate()
    {
    }

    public override void ExitState()
    {
        StopAllCoroutines();
    }
    #endregion
    //---------------------------------------------------------------------------------------------------

    private IEnumerator Teleport()
    {
        Material savedEyeMaterial = _eyeMesh.material;
        Material savedHeadMaterial = _headMesh.material;

        _eyeMesh.material = _eyeTpMaterial;
        _headMesh.material = _headTpMaterial;

        AudioManager.Instance.PlayOneShotOccluded(_monsterTPSound, MonsterTransform);
        yield return new WaitForSeconds(2.5f);
        MonsterTransform.position = TeleportDestination();
        AudioManager.Instance.PlayOneShotOccluded(_monsterTPSound, MonsterTransform);
        yield return new WaitForSeconds(1.5f);

        _eyeMesh.material = savedEyeMaterial;
        _headMesh.material = savedHeadMaterial;

        _stateMachine.ChangeState(_patrolingPointState);
    }

    protected virtual Vector3 TeleportDestination()
    {
        Vector3 tpDestination = _stateMachine.RandomDifferentPositionPoint();
        tpDestination.y = MonsterTransform.position.y;

        return tpDestination;
    }
}

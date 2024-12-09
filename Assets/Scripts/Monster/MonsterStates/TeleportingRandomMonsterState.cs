using FMOD.Studio;
using FMODUnity;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class TeleportingRandomMonsterState : MonsterState
{
    [SerializeField] private EventReference _monsterTPCastingSound;
    [SerializeField] private EventReference _monsterTPDoneSound;
    [SerializeField] private Light _lightCone;
    [SerializeField] private MeshRenderer _eyeMesh;
    [SerializeField] private MeshRenderer _headMesh;
    [SerializeField] private Material _eyeTpMaterial;
    [SerializeField] private Material _headTpMaterial;
    [HorizontalLine, Header("Next states")]
    [SerializeField] private PatrolingPointMonsterState _patrolingPointState;

    private EventInstance _castingSound;

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
        _castingSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        StopAllCoroutines();
    }
    #endregion
    //---------------------------------------------------------------------------------------------------

    private IEnumerator Teleport()
    {
        Material savedEyeMaterial = _eyeMesh.material;
        Material savedHeadMaterial = _headMesh.material;
        Color savedColor = _lightCone.color;

        _eyeMesh.material = _eyeTpMaterial;
        _headMesh.material = _headTpMaterial;
        _lightCone.color = Color.yellow;

        _castingSound = AudioManager.Instance.PlayOneShotOccluded(_monsterTPCastingSound, MonsterTransform);
        yield return new WaitForSeconds(2.5f);
        _castingSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        MonsterTransform.position = TeleportDestination();
        AudioManager.Instance.PlayOneShotOccluded(_monsterTPDoneSound, MonsterTransform);
        yield return new WaitForSeconds(1.5f);

        _eyeMesh.material = savedEyeMaterial;
        _headMesh.material = savedHeadMaterial;
        _lightCone.color = savedColor;

        _stateMachine.ChangeState(_patrolingPointState);
    }

    protected virtual Vector3 TeleportDestination()
    {
        Vector3 tpDestination = _stateMachine.RandomDifferentPositionPoint();
        tpDestination.y = MonsterTransform.position.y;

        return tpDestination;
    }
}

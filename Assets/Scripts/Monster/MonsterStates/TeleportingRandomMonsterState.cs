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
    private Material _savedEyeMaterial;
    private Material _savedHeadMaterial;
    private Color _savedColor;

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
        LoadMaterials();
        StopAllCoroutines();
    }
    #endregion
    //---------------------------------------------------------------------------------------------------

    private IEnumerator Teleport()
    {
        _savedEyeMaterial = _eyeMesh.material;
        _savedHeadMaterial = _headMesh.material;
        _savedColor = _lightCone.color;

        _eyeMesh.material = _eyeTpMaterial;
        _headMesh.material = _headTpMaterial;
        _lightCone.color = Color.yellow;

        _castingSound = AudioManager.Instance.PlayOneShotOccluded(_monsterTPCastingSound, MonsterTransform);
        yield return new WaitForSeconds(2.5f);
        _castingSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        MonsterTransform.position = TeleportDestination();
        AudioManager.Instance.PlayOneShotOccluded(_monsterTPDoneSound, MonsterTransform);
        yield return new WaitForSeconds(1.5f);

        LoadMaterials();

        _stateMachine.ChangeState(_patrolingPointState);
    }

    private void LoadMaterials()
    {
        _eyeMesh.material = _savedEyeMaterial;
        _headMesh.material = _savedHeadMaterial;
        _lightCone.color = _savedColor;
    }

    protected virtual Vector3 TeleportDestination()
    {
        Vector3 tpDestination = _stateMachine.RandomDifferentPositionPoint();
        tpDestination.y = MonsterTransform.position.y;

        return tpDestination;
    }
}

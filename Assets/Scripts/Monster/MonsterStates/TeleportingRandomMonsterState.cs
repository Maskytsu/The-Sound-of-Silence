using FMOD.Studio;
using FMODUnity;
using NaughtyAttributes;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TeleportingRandomMonsterState : MonsterState
{
    public event Action OnTpDestinationReached;

    [SerializeField] private Light _lightCone;
    [SerializeField] private MeshRenderer _eyeMesh;
    [SerializeField] private MeshRenderer _headMesh;
    [SerializeField] private Material _eyeTpMaterial;
    [SerializeField] private Material _headTpMaterial;
    [SerializeField] private Color _teleportingLightColor;
    [HorizontalLine, Header("Next states")]
    [SerializeField] private PatrolingPointMonsterState _patrolingPointState;

    private EventInstance _castingSound;
    private Material _savedEyeMaterial;
    private Material _savedHeadMaterial;
    private Color _savedColor;
    private float _savedIntensity;
    private Vector3 _savedPosition;

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
        OnTpDestinationReached = null;

        _castingSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        LoadMonsterLook();
        StopAllCoroutines();
    }
    #endregion
    //---------------------------------------------------------------------------------------------------

    private IEnumerator Teleport()
    {
        SaveAndSwapMonsterLook();

        _castingSound = AudioManager.Instance.PlayOneShotOccludedRI(FmodEvents.Instance.H_OCC_MonsterTPCast, MonsterTransform);
        yield return new WaitForSeconds(2.5f);
        _castingSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        MonsterTransform.position = TeleportDestination();
        AudioManager.Instance.PlayOneShotOccludedRI(FmodEvents.Instance.H_OCC_MonsterTPDone, MonsterTransform);
        yield return null;
        OnTpDestinationReached?.Invoke();
        yield return new WaitForSeconds(1.5f);

        LoadMonsterLook();

        _stateMachine.ChangeState(_patrolingPointState);
    }

    protected virtual Vector3 TeleportDestination()
    {
        Vector3 tpDestination = _stateMachine.RandomDifferentPositionPoint();
        tpDestination.y = MonsterTransform.position.y;

        return tpDestination;
    }

    private void SaveAndSwapMonsterLook()
    {
        _savedEyeMaterial = _eyeMesh.material;
        _savedHeadMaterial = _headMesh.material;
        _savedColor = _lightCone.color;
        _savedIntensity = _lightCone.intensity;
        _savedPosition = _lightCone.transform.localPosition;

        _eyeMesh.material = _eyeTpMaterial;
        _headMesh.material = _headTpMaterial;

        _lightCone.color = _teleportingLightColor;
        _lightCone.intensity = 2f;
        Vector3 newPos = _lightCone.transform.localPosition;
        newPos.z += 1f;
        _lightCone.transform.localPosition = newPos;
        _lightCone.type = LightType.Point;
    }

    private void LoadMonsterLook()
    {
        _eyeMesh.material = _savedEyeMaterial;
        _headMesh.material = _savedHeadMaterial;
        _lightCone.color = _savedColor;
        _lightCone.intensity = _savedIntensity;
        _lightCone.transform.localPosition = _savedPosition;

        _lightCone.type = LightType.Spot;
    }
}

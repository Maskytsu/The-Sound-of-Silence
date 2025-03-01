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
    [SerializeField] private Material _eyeTpMaterial;
    [SerializeField] private Color _teleportingLightColor;
    [HorizontalLine, Header("Next states")]
    [SerializeField] private PatrolingPointMonsterState _patrolingPointState;

    private EventInstance _castingSound;
    private Material _savedEyeMaterial;
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

        _castingSound = AudioManager.Instance.PlayOneShotOccludedRI(FmodEvents.Instance.OCC_MonsterTPCast, MonsterTransform);
        yield return new WaitForSeconds(2.5f);
        _castingSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        MonsterTransform.position = TeleportDestination();
        AudioManager.Instance.PlayOneShotOccludedRI(FmodEvents.Instance.OCC_MonsterTPDone, MonsterTransform);
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
        _savedColor = _lightCone.color;

        _eyeMesh.material = _eyeTpMaterial;
        _lightCone.color = _teleportingLightColor;
    }

    private void LoadMonsterLook()
    {
        _eyeMesh.material = _savedEyeMaterial;
        _lightCone.color = _savedColor;
    }
}

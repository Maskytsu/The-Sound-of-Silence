using Cinemachine;
using DG.Tweening;
using FMODUnity;
using System;
using System.Collections;
using UnityEngine;

public class RegularWakeUpSequence : MonoBehaviour
{
    public Action OnAnimationEnd;

    [Header("Scene Objects")]
    [SerializeField] private CinemachineVirtualCamera _lyingInBedCamera;
    [SerializeField] private Crutches _crutches;
    [SerializeField] private HearingAid _hearingAid;
    [SerializeField] private PlayerTargetTransform _standingPTT;

    private PlayerObjects PlayerObjects => PlayerObjects.Instance;

    private void Start()
    {
        UIManager.Instance.OnHourDisplayEnd += () => StartCoroutine(GetUp());
        UIManager.Instance.OnHourDisplayEnd += InputProvider.Instance.TurnOnGameplayOverlayMap;

        _crutches.OnInteract += () => StartCoroutine(StandUp());
        _hearingAid.OnInteract += () => StartCoroutine(StandUp());
    }

    private IEnumerator GetUp()
    {
        yield return new WaitForSeconds(2f);

        PlayerObjects.PlayerVirtualCamera.enabled = true;
        _lyingInBedCamera.enabled = false;
        yield return null;

        RuntimeManager.PlayOneShot(FmodEvents.Instance.BedGettingUp);
        while (CameraManager.Instance.CameraBrain.IsBlending)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        InputProvider.Instance.TurnOnPlayerCameraMap();
    }

    private IEnumerator StandUp()
    {
        if (_crutches.gameObject.activeSelf || _hearingAid.gameObject.activeSelf) yield break;

        InputProvider.Instance.TurnOffPlayerCameraMap();
        yield return new WaitForSeconds(0.5f);

        RuntimeManager.PlayOneShot(FmodEvents.Instance.StandingUp);
        PlayerObjects.Instance.Player.transform.DOMove(_standingPTT.Position, 2f).SetEase(Ease.InOutSine);
        yield return StartCoroutine(PlayerObjects.PlayerMovement.RotateCharacterAnimation(_standingPTT.Rotation, 3f));

        PlayerObjects.Instance.PlayerMovement.SetCharacterController(true);
        yield return new WaitForSeconds(0.5f);

        InputProvider.Instance.TurnOnPlayerMaps();
        yield return new WaitForSeconds(1f);

        OnAnimationEnd?.Invoke();
    }
}

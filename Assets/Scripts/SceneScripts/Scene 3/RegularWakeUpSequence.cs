using Cinemachine;
using DG.Tweening;
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
    [Header("Parameters")]
    [SerializeField] private Vector3 _playerStandingPos = new Vector3(22.5f, 8.105f, 23.5f);
    [SerializeField] private Vector3 _playerStandingRot = new Vector3(0f, 250f, 0f);

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

        PlayerObjects.Instance.Player.transform.DOMove(_playerStandingPos, 2f).SetEase(Ease.InOutSine);
        yield return StartCoroutine(PlayerObjects.PlayerMovement.RotateCharacterAnimation(_playerStandingRot, 3f));

        PlayerObjects.Instance.PlayerMovement.SetCharacterController(true);
        yield return new WaitForSeconds(0.5f);

        InputProvider.Instance.TurnOnPlayerMaps();
        yield return new WaitForSeconds(1f);

        OnAnimationEnd?.Invoke();
    }
}

using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class WakeUpSequence : MonoBehaviour
{
    public Action OnAnimationEnd;

    [Header("Scene Objects")]
    [SerializeField] private CinemachineVirtualCamera _lyingInBedCamera;
    [SerializeField] private Crutches _crutches;
    [SerializeField] private HearingAid _hearingAid;
    [Header("Parameters")]
    [SerializeField] private Vector3 _playerTargetPos;

    private bool _crutchesPickedUp = false;
    private bool _hearingAidPickedUp = false;

    private void Start()
    {
        UIManager.Instance.OnHourDisplayEnd += InputProvider.Instance.TurnOffPlayerMaps;
        UIManager.Instance.OnHourDisplayEnd += () => StartCoroutine(GetUp());

        _crutches.OnInteract += () =>
        {
            _crutchesPickedUp = true;
            StartCoroutine(StandUp());
        };

        _hearingAid.OnInteract += () =>
        {
            _hearingAidPickedUp = true;
            StartCoroutine(StandUp());
        };
    }


    private IEnumerator GetUp()
    {
        yield return new WaitForSeconds(1f);

        PlayerManager.Instance.VirtualMainCamera.enabled = true;
        _lyingInBedCamera.enabled = false;

        yield return null;

        while (PlayerManager.Instance.CameraBrain.IsBlending)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        InputProvider.Instance.TurnOnPlayerMainMap();
    }

    private IEnumerator StandUp()
    {
        if (!_crutchesPickedUp || !_hearingAidPickedUp) yield break;

        InputProvider.Instance.TurnOffPlayerMainMap();
        PlayerManager.Instance.PlayerVisuals.SetActive(true);

        Transform player = PlayerManager.Instance.Player.transform;
        Vector3 playerTargetRot = new Vector3(0, player.rotation.eulerAngles.y, 0);

        Tween moveTween = player.DOMove(_playerTargetPos, 2f).SetEase(Ease.InOutSine);
        Tween rotateTween = player.DORotate(playerTargetRot, 2f).SetEase(Ease.InOutSine);

        while (moveTween.IsActive() || rotateTween.IsActive())
        {
            yield return null;
        }

        PlayerManager.Instance.PlayerCharacterController.enabled = true;

        yield return new WaitForSeconds(0.5f);

        InputProvider.Instance.TurnOnPlayerMaps();

        yield return new WaitForSeconds(1f);

        OnAnimationEnd?.Invoke();
    }
}

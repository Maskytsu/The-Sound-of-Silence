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
    [SerializeField] private Crutches _crutchesHitbox;
    [SerializeField] private HearingAid _hearingAidHitbox;
    [Header("Parameters")]
    [SerializeField] private Vector3 _playerStandingPos = new Vector3(22.5f, 8.105f, 23.5f);
    [SerializeField] private float _playerStandingRotY = 200f;

    private bool _crutchesPickedUp = false;
    private bool _hearingAidPickedUp = false;

    private void Start()
    {
        UIManager.Instance.OnHourDisplayEnd += InputProvider.Instance.TurnOffPlayerMaps;
        UIManager.Instance.OnHourDisplayEnd += () => StartCoroutine(GetUp());

        _crutchesHitbox.OnInteract += () =>
        {
            _crutchesPickedUp = true;
            StartCoroutine(StandUp());
        };

        _hearingAidHitbox.OnInteract += () =>
        {
            _hearingAidPickedUp = true;
            StartCoroutine(StandUp());
        };
    }


    private IEnumerator GetUp()
    {
        yield return new WaitForSeconds(2f);

        PlayerManager.Instance.PlayerVirtualCamera.enabled = true;
        _lyingInBedCamera.enabled = false;

        yield return null;

        while (CameraManager.Instance.CameraBrain.IsBlending)
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
        Vector3 targetRotation = new Vector3(0, _playerStandingRotY, 0);

        Tween moveTween = player.DOMove(_playerStandingPos, 2f).SetEase(Ease.InOutSine);
        Tween rotateTween = player.DORotate(targetRotation, 2f).SetEase(Ease.InOutSine);

        while (moveTween.IsActive() )
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

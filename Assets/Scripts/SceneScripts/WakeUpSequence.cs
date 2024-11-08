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
    [SerializeField] private Vector3 _playerStandingRot = new Vector3(0f, 200f, 0f);

    private bool _crutchesPickedUp = false;
    private bool _hearingAidPickedUp = false;

    private PlayerManager PlayerManager => PlayerManager.Instance;

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

        PlayerManager.PlayerVirtualCamera.enabled = true;
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
        PlayerManager.PlayerVisuals.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        Transform player = PlayerManager.Instance.Player.transform;

        Tween moveTween = player.DOMove(_playerStandingPos, 2f).SetEase(Ease.InOutSine);

        yield return StartCoroutine(PlayerManager.PlayerMovement.RotateCharacter(_playerStandingRot, 3f));

        PlayerManager.Instance.PlayerCharacterController.enabled = true;

        yield return new WaitForSeconds(0.5f);

        InputProvider.Instance.TurnOnPlayerMaps();

        yield return new WaitForSeconds(1f);

        OnAnimationEnd?.Invoke();
    }
}

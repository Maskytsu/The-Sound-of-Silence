using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class HospitalWakeUpSequence : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Blackout _blackoutPrefab;
    [Header("Scriptable Objects")]
    [SerializeField] private DialogueSequenceScriptable _smallMonsterDialogue;
    [Header("Scene Objects")]
    [SerializeField] private Transform _smallMonster;
    [SerializeField] private Transform _monsterNewPos;
    [SerializeField] private Door _doors;
    [SerializeField] private CinemachineVirtualCamera _lyingInBedCamera;
    [SerializeField] private CinemachineVirtualCamera _fastGetUpCamera;
    [SerializeField] private MonsterStateMachine _monsterStateMachine;
    [SerializeField] private Crutches _crutches;
    [SerializeField] private HearingAid _hearingAid;
    [SerializeField] private PlayerTargetTransform _standingPTT;
    [Header("Parameters")]
    [SerializeField] private float _monsterNewRotY = 70f;

    private float _blackoutTime = 1f;
    private float _fadingTime = 1.5f;

    private float _fastBlackoutTime = 0.5f;
    private float _fastFadingTime = 0.75f;

    private void Start()
    {
        SetupScene();

        _crutches.OnInteract += () => StartCoroutine(StandUp());
        _hearingAid.OnInteract += () => StartCoroutine(StandUp());

        _smallMonsterDialogue.OnDialogueEnd += () => StartCoroutine(GetUp());

        StartCoroutine(WakeUp());
    }

    private IEnumerator WakeUp()
    {
        if (SceneResetedChecker.Instance != null)
        {
            StartCoroutine(FastGetUp());
        }

        Blackout blackout = Instantiate(_blackoutPrefab);

        yield return new WaitForSeconds(_blackoutTime);
        Tween fadeTween = blackout.Image.DOFade(0f, _fadingTime);

        while (fadeTween.IsActive()) yield return null;
        Destroy(blackout.gameObject);
        InputProvider.Instance.TurnOnGameplayOverlayMap();

        if (SceneResetedChecker.Instance == null)
        {
            StartCoroutine(MonsterRunAway());
        }
    }

    private IEnumerator MonsterRunAway()
    {
        yield return new WaitForSeconds(1.5f);
        Vector3 newRot = _smallMonster.eulerAngles;
        newRot.y = _monsterNewRotY;
        _smallMonster.DORotate(newRot, 1.5f);

        yield return new WaitForSeconds(1f);
        Tween moveTween = _smallMonster.DOMove(_monsterNewPos.position, 3f);

        while (moveTween.IsPlaying()) yield return null;
        _smallMonster.gameObject.SetActive(false);
        _doors.SwitchDoorAnimated();

        yield return new WaitForSeconds(1f);
        UIManager.Instance.DisplayDialogueSequence(_smallMonsterDialogue);
    }

    private IEnumerator FastGetUp()
    {
        yield return new WaitForSeconds(_blackoutTime);
        _fastGetUpCamera.enabled = true;
        _lyingInBedCamera.enabled = false;
        yield return null;

        while (CameraManager.Instance.CameraBrain.IsBlending) yield return null;

        yield return new WaitForSeconds(1f);
        PlayerObjects.Instance.PlayerVirtualCamera.enabled = true;
        _fastGetUpCamera.enabled = false;
        yield return null;

        while (CameraManager.Instance.CameraBrain.IsBlending) yield return null;
        yield return new WaitForSeconds(0.2f);
        InputProvider.Instance.TurnOnPlayerCameraMap();
    }

    private IEnumerator GetUp()
    {
        yield return new WaitForSeconds(1f);
        PlayerObjects.Instance.PlayerVirtualCamera.enabled = true;
        _lyingInBedCamera.enabled = false;
        yield return null;

        while (CameraManager.Instance.CameraBrain.IsBlending) yield return null;
        yield return new WaitForSeconds(0.2f);
        InputProvider.Instance.TurnOnPlayerCameraMap();
    }

    private IEnumerator StandUp()
    {
        if (_crutches.gameObject.activeSelf || _hearingAid.gameObject.activeSelf) yield break;

        _monsterStateMachine.MonsterTransform.gameObject.SetActive(true);
        InputProvider.Instance.TurnOffPlayerCameraMap();

        yield return new WaitForSeconds(0.5f);
        PlayerObjects.Instance.Player.transform.DOMove(_standingPTT.Position, 1.5f).SetEase(Ease.InOutSine);

        yield return StartCoroutine(PlayerObjects.Instance.PlayerMovement.RotateCharacterAnimation(_standingPTT.Rotation, 2f));
        PlayerObjects.Instance.PlayerMovement.SetCharacterController(true);

        yield return new WaitForSeconds(0.5f);
        InputProvider.Instance.TurnOnPlayerMaps();
    }

    private void SetupScene()
    {
        if (SceneResetedChecker.Instance != null)
        {
            _doors.SetOpened(false);
            _smallMonster.gameObject.SetActive(false);
            _blackoutTime = _fastBlackoutTime;
            _fadingTime = _fastFadingTime;
        }
    }
}
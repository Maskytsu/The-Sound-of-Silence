using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private Crutches _crutches;
    [SerializeField] private HearingAid _hearingAid;
    [SerializeField] private PlayerTargetTransform _standingPTT;
    [Header("Parameters")]
    [SerializeField] private float _monsterNewRotY = 70f;

    private float _blackoutTime = 1f;
    private float _fadingTime = 1.5f;


    private void Start()
    {
        _crutches.OnInteract += () => StartCoroutine(StandUp());
        _hearingAid.OnInteract += () => StartCoroutine(StandUp());

        StartCoroutine(WakeUp());
    }

    private IEnumerator WakeUp()
    {
        Blackout blackout = Instantiate(_blackoutPrefab);

        yield return new WaitForSeconds(_blackoutTime);
        Tween fadeTween = blackout.Image.DOFade(0f, _fadingTime);

        while (fadeTween.IsActive()) yield return null;
        Destroy(blackout.gameObject);
        InputProvider.Instance.TurnOnGameplayOverlayMap();

        StartCoroutine(MonsterRunAway());
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
        _doors.InteractionHitbox.OnInteract?.Invoke();

        yield return new WaitForSeconds(0.5f);
        UIManager.Instance.DisplayDialogueSequence(_smallMonsterDialogue);
        _smallMonsterDialogue.OnDialogueEnd += () => StartCoroutine(GetUp());
    }

    private IEnumerator GetUp()
    {
        yield return new WaitForSeconds(2f);
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

        InputProvider.Instance.TurnOffPlayerCameraMap();

        yield return new WaitForSeconds(0.5f);
        PlayerObjects.Instance.Player.transform.DOMove(_standingPTT.Position, 1.5f).SetEase(Ease.InOutSine);

        yield return StartCoroutine(PlayerObjects.Instance.PlayerMovement.RotateCharacterAnimation(_standingPTT.Rotation, 2f));
        PlayerObjects.Instance.PlayerMovement.SetCharacterController(true);

        yield return new WaitForSeconds(0.5f);
        InputProvider.Instance.TurnOnPlayerMaps();
    }
}

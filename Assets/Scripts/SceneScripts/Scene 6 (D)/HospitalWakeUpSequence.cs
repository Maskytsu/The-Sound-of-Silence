using Cinemachine;
using DG.Tweening;
using FMODUnity;
using System.Collections;
using UnityEngine;

public class HospitalWakeUpSequence : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private DialogueSequenceScriptable _smallMonsterDialogue;
    [Header("Scene Objects")]
    [SerializeField] private Transform _smallMonster;
    [SerializeField] private Transform _monsterNewPos;
    [SerializeField] private Door _doors;
    [SerializeField] private CinemachineVirtualCamera _lyingInBedCamera;
    [SerializeField] private CinemachineVirtualCamera _fastGetUpCamera;
    [SerializeField] private Crutches _crutches;
    [SerializeField] private HearingAid _hearingAid;
    [SerializeField] private PlayerTargetTransform _standingPTT;
    [Header("Parameters")]
    [SerializeField] private float _monsterNewRotY = 70f;

    private float _blackoutTime = 1f;

    private void Start()
    {
        _crutches.OnInteract += () => StartCoroutine(StandUp());
        _hearingAid.OnInteract += () => StartCoroutine(StandUp());
        _hearingAid.OnInteract += () => MonsterStateMachine.Instance.MonsterTransform.gameObject.SetActive(true);

        _smallMonsterDialogue.OnDialogueEnd += () => StartCoroutine(GetUp());

        StartCoroutine(WakeUp());
    }

    private IEnumerator WakeUp()
    {
        HUD.Instance.Blink.SetActiveBlackout(true);
        yield return new WaitForSeconds(_blackoutTime);

        HUD.Instance.Blink.PlayOpenEyes(1.0f);
        while (HUD.Instance.Blink.IsPlaying) yield return null;

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
        _doors.SwitchDoorAnimated();

        yield return new WaitForSeconds(1f);
        DialogueManager.Instance.DisplayDialogue(_smallMonsterDialogue);
    }

    private IEnumerator GetUp()
    {
        yield return new WaitForSeconds(1f);
        PlayerObjects.Instance.PlayerVirtualCamera.enabled = true;
        _lyingInBedCamera.enabled = false;
        yield return null;

        RuntimeManager.PlayOneShot(FmodEvents.Instance.BedGettingUp);
        while (CameraManager.Instance.CameraBrain.IsBlending) yield return null;
        yield return new WaitForSeconds(0.2f);
        InputProvider.Instance.TurnOnPlayerCameraMap();
    }

    private IEnumerator StandUp()
    {
        if (_crutches.gameObject.activeSelf || _hearingAid.gameObject.activeSelf) yield break;

        InputProvider.Instance.TurnOffPlayerCameraMap();

        yield return new WaitForSeconds(0.5f);
        RuntimeManager.PlayOneShot(FmodEvents.Instance.StandingUp);
        PlayerObjects.Instance.Player.transform.DOMove(_standingPTT.Position, 1.5f).SetEase(Ease.InOutSine);

        yield return StartCoroutine(PlayerObjects.Instance.PlayerMovement.RotateCharacterAnimation(_standingPTT.Rotation, 2f));
        PlayerObjects.Instance.PlayerMovement.SetCharacterController(true);

        yield return new WaitForSeconds(0.5f);
        InputProvider.Instance.TurnOnPlayerMaps();
    }
}
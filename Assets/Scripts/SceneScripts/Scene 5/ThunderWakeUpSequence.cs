using Cinemachine;
using DG.Tweening;
using FMODUnity;
using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;

public class ThunderWakeUpSequence : MonoBehaviour
{
    public Action OnAnimationEnd;

    [Header("Prefabs")]
    [SerializeField] private Blackout _whiteBlackoutPrefab;
    [Header("Scriptable Objects")]
    [SerializeField] private DialogueSequenceScriptable _hearingAidTookPillsDialogue;
    [SerializeField] private DialogueSequenceScriptable _hearingAidNoPillsDialogue;
    [Header("Scene Objects")]
    [SerializeField] private CinemachineVirtualCamera _lyingInBedCamera;
    [SerializeField] private Crutches _crutches;
    [SerializeField] private Light _lampLight;
    [SerializeField] private PlayerTargetTransform _standingPTT;

    private PlayerObjects PlayerObjectsHolder => PlayerObjects.Instance;

    private void Start()
    {
        StartCoroutine(LightningEffect());

        UIManager.Instance.OnHourDisplayEnd += () => StartCoroutine(GetUp());
        UIManager.Instance.OnHourDisplayEnd += InputProvider.Instance.TurnOnGameplayOverlayMap;

        _crutches.OnInteract += () => StartCoroutine(StandUp());
    }

    private IEnumerator LightningEffect()
    {
        Blackout blackout = Instantiate(_whiteBlackoutPrefab);

        yield return new WaitForSeconds(0.25f);
        Tween fadeBlackoutTween = blackout.Image.DOFade(0f, 0.25f);

        while (fadeBlackoutTween.IsPlaying())
        {
            yield return null;
        }

        float startingValue = RenderSettings.ambientIntensity;
        float alphaValue = startingValue;
        float targetValue = 1f;
        float fadeSpeed = 0.25f;
        float distanceBetween = Mathf.Abs(startingValue - targetValue);

        while (RenderSettings.ambientIntensity > targetValue)
        {
            alphaValue -= startingValue * (Time.deltaTime / fadeSpeed);
            RenderSettings.ambientIntensity = alphaValue;
            yield return null;
        }
        RenderSettings.ambientIntensity = targetValue;

        yield return new WaitForSeconds(2f);
        _lampLight.gameObject.SetActive(false);
        GameManager.Instance.ChangeElectricityState(false);
    }
    
    private IEnumerator GetUp()
    {
        yield return new WaitForSeconds(2f);

        PlayerObjectsHolder.PlayerVirtualCamera.enabled = true;
        _lyingInBedCamera.enabled = false;

        yield return null;

        RuntimeManager.PlayOneShot(FmodEvents.Instance.BedGettingUp);
        while (CameraManager.Instance.CameraBrain.IsBlending)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        DialogueSequenceScriptable dialogue;
        if (GameState.Instance.TookPills) dialogue = _hearingAidTookPillsDialogue;
        else dialogue = _hearingAidNoPillsDialogue;

        UIManager.Instance.DisplayDialogueSequence(dialogue);
        dialogue.OnDialogueEnd += InputProvider.Instance.TurnOnPlayerCameraMap;
    }

    private IEnumerator StandUp()
    {
        InputProvider.Instance.TurnOffPlayerCameraMap();
        yield return new WaitForSeconds(0.5f);

        RuntimeManager.PlayOneShot(FmodEvents.Instance.StandingUp);
        Transform player = PlayerObjects.Instance.Player.transform;
        player.DOMove(_standingPTT.Position, 2f).SetEase(Ease.InOutSine);

        yield return StartCoroutine(PlayerObjectsHolder.PlayerMovement.RotateCharacterAnimation(_standingPTT.Rotation, 3f));

        PlayerObjects.Instance.PlayerMovement.SetCharacterController(true);
        yield return new WaitForSeconds(0.5f);

        InputProvider.Instance.TurnOnPlayerMaps();
        yield return new WaitForSeconds(1f);

        OnAnimationEnd?.Invoke();
    }
}

using Cinemachine;
using DG.Tweening;
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
    [Header("Parameters")]
    [SerializeField] private Vector3 _playerStandingPos = new Vector3(22.5f, 8.105f, 23.5f);
    [SerializeField] private Vector3 _playerStandingRot = new Vector3(0f, 200f, 0f);

    private PlayerObjectsHolder PlayerManager => PlayerObjectsHolder.Instance;

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
        Tween fadeBlackoutTween = blackout.Image.DOFade(0f, 1f);

        yield return new WaitForSeconds(0.5f);

        float startingValue = RenderSettings.ambientIntensity;
        float alphaValue = startingValue;
        float fadeSpeed = 0.5f;

        while (RenderSettings.ambientIntensity > 1f)
        {
            alphaValue -= (Time.deltaTime / fadeSpeed) * startingValue;
            RenderSettings.ambientIntensity = alphaValue;
            yield return null;
        }
        RenderSettings.ambientIntensity = 1f;

        yield return new WaitForSeconds(1f);
        StartCoroutine(TurnOffElectricity());
    }

    private IEnumerator TurnOffElectricity()
    {
        yield return null;
        _lampLight.gameObject.SetActive(false);
        GameManager.Instance.ChangeElectricityState(false);
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

        Transform player = PlayerObjectsHolder.Instance.Player.transform;

        Tween moveTween = player.DOMove(_playerStandingPos, 2f).SetEase(Ease.InOutSine);
        yield return StartCoroutine(PlayerManager.PlayerMovement.RotateCharacter(_playerStandingRot, 3f));

        PlayerObjectsHolder.Instance.PlayerCharacterController.enabled = true;
        yield return new WaitForSeconds(0.5f);

        InputProvider.Instance.TurnOnPlayerMaps();
        yield return new WaitForSeconds(1f);

        OnAnimationEnd?.Invoke();
    }
}

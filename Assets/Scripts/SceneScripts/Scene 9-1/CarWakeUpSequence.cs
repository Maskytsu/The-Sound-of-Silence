using Cinemachine;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarWakeUpSequence : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _checkOutsideQuest;
    [SerializeField] private DialogueSequenceScriptable _arrivingCarDialogue;
    [Header("Scene Objects")]
    [SerializeField] private CinemachineVirtualCamera _lyingInBedCamera;
    [SerializeField] private Crutches _crutches;
    [SerializeField] private PlayerTargetTransform _standingPTT;
    [SerializeField] private Transform _carSoundPosition;

    private void Start()
    {
        RuntimeManager.PlayOneShotAttached(FmodEvents.Instance.SPT_ComingCar, _carSoundPosition.gameObject);

        UIManager.Instance.OnHourDisplayEnd += () => StartCoroutine(EndCarSoundAndPlayDialogue());

        _arrivingCarDialogue.OnDialogueEnd += () => StartCoroutine(GetUp());

        _crutches.OnInteract += () => StartCoroutine(StandUp());
    }

    private IEnumerator EndCarSoundAndPlayDialogue()
    {
        InputProvider.Instance.TurnOnGameplayOverlayMap();
        yield return new WaitForSeconds(2.5f);
        UIManager.Instance.DisplayDialogueSequence(_arrivingCarDialogue);
    }

    private IEnumerator GetUp()
    {
        yield return new WaitForSeconds(2f);

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
        InputProvider.Instance.TurnOffPlayerCameraMap();

        yield return new WaitForSeconds(0.5f);
        RuntimeManager.PlayOneShot(FmodEvents.Instance.StandingUp);
        PlayerObjects.Instance.Player.transform.DOMove(_standingPTT.Position, 1.5f).SetEase(Ease.InOutSine);

        yield return StartCoroutine(PlayerObjects.Instance.PlayerMovement.RotateCharacterAnimation(_standingPTT.Rotation, 2f));
        PlayerObjects.Instance.PlayerMovement.SetCharacterController(true);

        yield return new WaitForSeconds(0.5f);
        InputProvider.Instance.TurnOnPlayerMaps();

        yield return new WaitForSeconds(2f);
        QuestManager.Instance.StartQuest(_checkOutsideQuest);
    }
}

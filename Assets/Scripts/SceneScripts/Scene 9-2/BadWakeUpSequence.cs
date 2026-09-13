using Cinemachine;
using DG.Tweening;
using FMODUnity;
using System.Collections;
using UnityEngine;

public class BadWakeUpSequence : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _goToGarageQuest;
    [SerializeField] private DialogueSequenceScriptable _dialogueSequence;
    [Header("Scene Objects")]
    [SerializeField] private CinemachineVirtualCamera _lyingInBedCamera;
    [SerializeField] private Crutches _crutches;
    [SerializeField] private PlayerTargetTransform _standingPTT;

    private void Start()
    {
        UIManager.Instance.OnHourDisplayEnd += () => StartCoroutine(GetUp());
        UIManager.Instance.OnHourDisplayEnd += InputProvider.Instance.TurnOnGameplayOverlayMap;

        _crutches.OnInteract += () => StartCoroutine(StandUp());
    }

    private IEnumerator GetUp()
    {
        yield return new WaitForSeconds(1.5f);

        DialogueManager.Instance.DisplayDialogue(_dialogueSequence);
        yield return new WaitForSeconds(0.8f * _dialogueSequence.GetDialogueDuration());

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
        QuestManager.Instance.StartQuest(_goToGarageQuest);
    }
}

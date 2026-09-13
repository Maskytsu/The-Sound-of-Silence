using Cinemachine;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseToiletQuestHandler : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _useToiletQuest;
    [SerializeField] private DialogueSequenceScriptable _dialogueSequence;
    [Header("Scene Objects")]
    [SerializeField] private RegularWakeUpSequence _wakeUpSequence;
    [SerializeField] private Toilet _toilet;
    [SerializeField] private Transform _soundPoint;
    [SerializeField] private CinemachineVirtualCamera _peeCamera;
    [SerializeField] private PlayerTargetTransform _beforPeePTT;

    private void Start()
    {
        _wakeUpSequence.OnAnimationEnd += () => QuestManager.Instance.StartQuest(_useToiletQuest);

        _toilet.OnCoverOpened += () => StartCoroutine(PeeAnimation());
    }

    private IEnumerator PeeAnimation()
    {
        yield return new WaitForSeconds(0.5f);

        Transform player = PlayerObjects.Instance.Player.transform;
        PlayerMovement playerMovement = PlayerObjects.Instance.PlayerMovement;

        Tween moveTween = player.DOMove(_beforPeePTT.Position, 2f).SetEase(Ease.InOutSine);
        yield return StartCoroutine(playerMovement.RotateCharacterAnimation(_beforPeePTT.Rotation, 2f));

        _peeCamera.enabled = true;
        PlayerObjects.Instance.PlayerVirtualCamera.enabled = false;

        yield return null;
        while (CameraManager.Instance.CameraBrain.IsBlending) yield return null;

        RuntimeManager.PlayOneShot(FmodEvents.Instance.Peeing);
        yield return new WaitForSeconds(AudioManager.EventLength(FmodEvents.Instance.Peeing));
        yield return new WaitForSeconds(0.1f);

        PlayerObjects.Instance.PlayerVirtualCamera.enabled = true;
        _peeCamera.enabled = false;

        yield return new WaitForSeconds(0.2f);
        RuntimeManager.PlayOneShotAttached(FmodEvents.Instance.SPT_OpeningWindow, _soundPoint.gameObject);

        yield return new WaitForSeconds(1f);
        DialogueManager.Instance.DisplayDialogue(_dialogueSequence);

        while (CameraManager.Instance.CameraBrain.IsBlending) yield return null;

        QuestManager.Instance.EndQuest(_useToiletQuest);
        InputProvider.Instance.TurnOnPlayerMaps();
    }
}

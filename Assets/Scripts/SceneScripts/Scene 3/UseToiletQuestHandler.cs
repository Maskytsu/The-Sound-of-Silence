using Cinemachine;
using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseToiletQuestHandler : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _useToiletQuest;
    [Header("Scene Objects")]
    [SerializeField] private WakeUpSequence _wakeUpSequence;
    [SerializeField] private Toilet _toilet;
    [SerializeField] private Transform _soundPoint;
    [SerializeField] private CinemachineVirtualCamera _peeCamera;
    [Header("Parameters")]
    [SerializeField] private EventReference _eventRef;
    [SerializeField] private Vector3 _playerBeforePeePos = new Vector3(21.82f, 8.105f, 7.857f);
    [SerializeField] private Vector3 _playerBeforePeeRot = new Vector3(0f, 0f, 0f);

    private EventInstance _audioEvent;

    private void Start()
    {
        _wakeUpSequence.OnAnimationEnd += () => QuestManager.Instance.StartQuest(_useToiletQuest);

        _toilet.OnCoverOpened += () => StartCoroutine(PeeAnimation());
    }

    private IEnumerator PeeAnimation()
    {
        yield return new WaitForSeconds(0.5f);

        Transform player = PlayerManager.Instance.Player.transform;
        PlayerMovement playerMovement = PlayerManager.Instance.PlayerMovement;

        Tween moveTween = player.DOMove(_playerBeforePeePos, 2f).SetEase(Ease.InOutSine);
        yield return StartCoroutine(playerMovement.RotateCharacter(_playerBeforePeeRot, 2f));

        _peeCamera.enabled = true;
        PlayerManager.Instance.PlayerVirtualCamera.enabled = false;

        yield return null;
        while (CameraManager.Instance.CameraBrain.IsBlending)
        {
            yield return null;
        }

        yield return new WaitForSeconds(3f);

        //pee sound

        PlayerManager.Instance.PlayerVirtualCamera.enabled = true;
        _peeCamera.enabled = false;

        yield return new WaitForSeconds(0.5f);
        _audioEvent = AudioManager.Instance.PlayOneShotOccluded(_eventRef, _soundPoint);

        while (CameraManager.Instance.CameraBrain.IsBlending)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        QuestManager.Instance.EndQuest(_useToiletQuest);

        PlayerManager.Instance.PlayerVisuals.SetActive(true);
        InputProvider.Instance.TurnOnPlayerMaps();
    }
}
using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SafeRoomWakeUpSequence : MonoBehaviour
{
    public event Action OnAnimationEnd;

    [Header("Prefabs")]
    [SerializeField] private Blackout _blackoutPrefab;
    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _escapeQuest;
    [SerializeField] private QuestScriptable _resetBreakersQuest;
    [SerializeField] private QuestScriptable _killQuest;
    [Header("Scene Objects")]
    [SerializeField] private CinemachineVirtualCamera _lyingInBedCamera;
    [SerializeField] private CinemachineVirtualCamera _fastGetUpCamera;
    [SerializeField] private Crutches _crutches;
    [SerializeField] private PickableItem _keys;
    [SerializeField] private PlayerTargetTransform _standingPTT;
    [SerializeField] private Scene7ResetHandler _sceneResetHandler;

    private float _fastBlackoutTime = 0.5f;
    private float _fastFadingTime = 0.75f;
    private bool _keysPickedUp = false;

    private void Start()
    {
        _crutches.OnInteract += () => StartCoroutine(StandUp());

        _keys.OnInteract += () =>
        {
            _keysPickedUp = true;
            StartCoroutine(StandUp());
        };

        OnAnimationEnd += () => StartCoroutine(QuestManager.Instance.StartQuestDelayed(_resetBreakersQuest));
        OnAnimationEnd += () => StartCoroutine(QuestManager.Instance.StartQuestDelayed(_escapeQuest));

        if (_sceneResetHandler.TookGun) OnAnimationEnd += () => StartCoroutine(QuestManager.Instance.StartQuestDelayed(_killQuest));

        StartCoroutine(WakeUp());
    }

    private IEnumerator WakeUp()
    {
        _lyingInBedCamera.enabled = true;

        StartCoroutine(FastGetUp());

        Blackout blackout = Instantiate(_blackoutPrefab);

        yield return new WaitForSeconds(_fastBlackoutTime);
        Tween fadeTween = blackout.Image.DOFade(0f, _fastFadingTime);

        while (fadeTween.IsActive()) yield return null;
        Destroy(blackout.gameObject);
        InputProvider.Instance.TurnOnGameplayOverlayMap();
    }

    private IEnumerator FastGetUp()
    {
        yield return new WaitForSeconds(_fastBlackoutTime);
        _fastGetUpCamera.enabled = true;
        _lyingInBedCamera.enabled = false;
        yield return null;

        while (CameraManager.Instance.CameraBrain.IsBlending) yield return null;

        yield return new WaitForSeconds(1.5f);
        PlayerObjects.Instance.PlayerVirtualCamera.enabled = true;
        _fastGetUpCamera.enabled = false;
        yield return null;

        while (CameraManager.Instance.CameraBrain.IsBlending) yield return null;
        yield return new WaitForSeconds(0.2f);
        InputProvider.Instance.TurnOnPlayerCameraMap();
    }

    private IEnumerator StandUp()
    {
        if (_crutches.gameObject.activeSelf || !_keysPickedUp) yield break;

        InputProvider.Instance.TurnOffPlayerCameraMap();

        yield return new WaitForSeconds(0.5f);
        PlayerObjects.Instance.Player.transform.DOMove(_standingPTT.Position, 1.5f).SetEase(Ease.InOutSine);

        yield return StartCoroutine(PlayerObjects.Instance.PlayerMovement.RotateCharacterAnimation(_standingPTT.Rotation, 2f));
        PlayerObjects.Instance.PlayerMovement.SetCharacterController(true);

        yield return new WaitForSeconds(0.5f);
        OnAnimationEnd?.Invoke();
        InputProvider.Instance.TurnOnPlayerMaps();
    }
}
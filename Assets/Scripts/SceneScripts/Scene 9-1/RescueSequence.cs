using Cinemachine;
using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RescueSequence : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Blackout _blackoutPrefab;
    [Header("Scriptable Objects")]
    [SerializeField] private DialogueSequenceScriptable _claireFinalDialogue;
    [Header("Scene Objects")]
    [SerializeField] private Trigger _playerTrigger;
    [SerializeField] private Door _door;
    [SerializeField] private PlayerTargetTransform _doorPTT;
    [SerializeField] private CinemachineVirtualCamera _hugCamera;
    [Header("Parameters")]
    [Scene, SerializeField] private string _nextScene;

    private float _fadingTime = 2f;

    private void Start()
    {
        _playerTrigger.OnObjectTriggerEnter += () => StartCoroutine(ClaireRescueSequence());
    }

    private IEnumerator ClaireRescueSequence()
    {
        InputProvider.Instance.TurnOffPlayerMaps();
        _playerTrigger.gameObject.SetActive(false);
        MonsterStateMachine.Instance.ChangeState<PerishingMonsterState>();

        Transform player = PlayerObjects.Instance.transform;
        Tween movePlayerTween = player.DOMove(_doorPTT.Position, 2f).SetSpeedBased().SetEase(Ease.InOutSine);
        yield return StartCoroutine(PlayerObjects.Instance.PlayerMovement.RotateCharacterAnimation(_doorPTT.Rotation, 2f));
        while (movePlayerTween.IsPlaying()) yield return null;

        _door.SwitchDoorAnimated();
        PlayerObjects.Instance.PlayerEquipment.ChangeItem(ItemType.NONE);

        yield return new WaitForSeconds(2f);

        _hugCamera.enabled = true;
        PlayerObjects.Instance.PlayerVirtualCamera.enabled = false;
        yield return null;
        while (CameraManager.Instance.CameraBrain.IsBlending) yield return null;
        yield return new WaitForSeconds(0.5f);

        DialogueManager.Instance.DisplayDialogue(_claireFinalDialogue);
        float halfOfDialogue = _claireFinalDialogue.GetDialogueDuration() * 0.5f;
        yield return new WaitForSeconds(halfOfDialogue);
        float savedTime = Time.time;

        InputProvider.Instance.TurnOffGameplayOverlayMap();

        HUD.Instance.Blink.PlayCloseEyes(1.0f);
        while (HUD.Instance.Blink.IsPlaying) yield return null;

        float remainingDialogueTime = halfOfDialogue - (Time.time - savedTime);
        if (remainingDialogueTime > 0) yield return new WaitForSeconds(remainingDialogueTime);

        yield return new WaitForSeconds(2f);
        GameManager.Instance.LoadSceneAndSaveGameState(_nextScene);
    }
}
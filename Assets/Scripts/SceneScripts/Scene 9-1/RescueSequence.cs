using Cinemachine;
using DG.Tweening;
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
    [SerializeField] private MonsterStateMachine _monsterSM;
    [SerializeField] private PerishingMonsterState _perishingState;
    [SerializeField] private PlayerTargetTransform _doorPTT;
    [SerializeField] private CinemachineVirtualCamera _hugCamera;

    private float _fadingTime = 2f;

    private void Start()
    {
        _playerTrigger.OnObjectTriggerEnter += () => StartCoroutine(ClaireRescueSequence());
    }

    private IEnumerator ClaireRescueSequence()
    {
        InputProvider.Instance.TurnOffPlayerMaps();
        _playerTrigger.gameObject.SetActive(false);
        _monsterSM.ChangeState(_perishingState);

        Transform player = PlayerObjects.Instance.transform;
        Tween movePlayerTween = player.DOMove(_doorPTT.Position, 2f).SetSpeedBased().SetEase(Ease.InOutSine);
        yield return StartCoroutine(PlayerObjects.Instance.PlayerMovement.RotateCharacterAnimation(_doorPTT.Rotation, 15f, true));
        while (movePlayerTween.IsPlaying()) yield return null;

        _door.SwitchDoorAnimated();
        PlayerObjects.Instance.PlayerEquipment.ChangeItem(ItemType.NONE);

        yield return new WaitForSeconds(1f);

        _hugCamera.enabled = true;
        PlayerObjects.Instance.PlayerVirtualCamera.enabled = false;
        yield return null;
        while (CameraManager.Instance.CameraBrain.IsBlending) yield return null;
        yield return new WaitForSeconds(0.5f);

        UIManager.Instance.DisplayDialogueSequence(_claireFinalDialogue);
        float halfOfDialogue = _claireFinalDialogue.DialogueDuration() * 0.5f;
        yield return new WaitForSeconds(halfOfDialogue);
        float savedTime = Time.time;

        InputProvider.Instance.TurnOffGameplayOverlayMap();
        Blackout blackoutBackground = Instantiate(_blackoutPrefab);
        blackoutBackground.SetAlphaToZero();
        Tween fadeTween = blackoutBackground.Image.DOFade(1, _fadingTime);
        while (fadeTween.IsActive()) yield return null;

        float remainingDialogueTime = halfOfDialogue - (Time.time - savedTime);
        if (remainingDialogueTime > 0) yield return new WaitForSeconds(remainingDialogueTime);

        yield return new WaitForSeconds(1f);
        Debug.Log("END OF SCENE!");
        //GameManager.Instance.LoadSceneAndSaveGameState(_nextScene);
    }
}
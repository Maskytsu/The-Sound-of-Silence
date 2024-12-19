using Cinemachine;
using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CouchEndingHandler : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Blackout _blackoutPrefab;
    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _couchQuest;
    [SerializeField] private DialogueSequenceScriptable _endingTVDialogue;
    [Header("Scene Objects")]
    [SerializeField] private Couch _couch;
    [SerializeField] private Crutches _crutches;
    [SerializeField] private CinemachineVirtualCamera _puttingOffCamera;
    [SerializeField] private CinemachineVirtualCamera _tvCamera;
    [SerializeField] private GameObject _TVPilot;
    [SerializeField] private MeshRenderer _TVScreen;
    [Header("Parameters")]
    [Scene, SerializeField] private string _nextScene;

    private float _fadingTime = 2f;

    private void Start()
    {
        _couchQuest.OnQuestStart += BeginCouchQuest;
    }

    private void BeginCouchQuest()
    {
        _couch.InteractionHitbox.gameObject.SetActive(true);
        _couch.OnInteract += () => StartCoroutine(TVSleepAnimation());
    }

    private IEnumerator TVSleepAnimation()
    {
        InputProvider.Instance.TurnOffPlayerMaps();
        PlayerObjects.Instance.PlayerEquipment.ChangeItem(ItemType.NONE);
        _couch.InteractionHitbox.gameObject.SetActive(false);

        _puttingOffCamera.enabled = true;
        PlayerObjects.Instance.PlayerVirtualCamera.enabled = false;
        yield return null;

        while (CameraManager.Instance.CameraBrain.IsBlending) yield return null;
        yield return new WaitForSeconds(0.5f);

        _crutches.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);

        _tvCamera.enabled = true;
        _puttingOffCamera.enabled = false;
        yield return null;

        while (CameraManager.Instance.CameraBrain.IsBlending) yield return null;

        yield return new WaitForSeconds(1f);

        _TVPilot.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        Tween fadeTVTween = _TVScreen.material.DOColor(new Color(255, 255, 255), 0.5f);

        yield return new WaitForSeconds(1f);

        UIManager.Instance.DisplayDialogueSequence(_endingTVDialogue);
        yield return new WaitForSeconds(0.5f * _endingTVDialogue.DialogueDuration());
        _endingTVDialogue.OnDialogueEnd += () => StartCoroutine(EndScene());

        InputProvider.Instance.TurnOffGameplayOverlayMap();
        Blackout blackoutBackground = Instantiate(_blackoutPrefab);
        blackoutBackground.SetAlphaToZero();

        Tween fadeTween = blackoutBackground.Image.DOFade(1, _fadingTime);
        while (fadeTween.IsActive()) yield return null;
    }

    private IEnumerator EndScene()
    {
        yield return new WaitForSeconds(1f);
        GameManager.Instance.LoadSceneAndSaveGameState(_nextScene);
    }
}

using Cinemachine;
using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedEndingHandler : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Blackout _blackoutPrefab;
    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _bedQuest;
    [Header("Scene Objects")]
    [SerializeField] private LightSwitch _sharonRoomLightSwitch;
    [SerializeField] private Bed _bed;
    [SerializeField] private Crutches _crutches;
    [SerializeField] private CinemachineVirtualCamera _puttingOffCamera;
    [SerializeField] private CinemachineVirtualCamera _lyingInBedCamera;
    [Header("Parameters")]
    [Scene, SerializeField] private string _nextScene;

    private float _fadingTime = 2f;

    private void Start()
    {
        _bedQuest.OnQuestStart += StartCheckingLight;
        _bed.OnInteract += () => StartCoroutine(SleepAnimation());
    }

    private void StartCheckingLight()
    {
        CheckLight();
        _sharonRoomLightSwitch.OnInteract += CheckLight;
    }

    private void CheckLight()
    {
        if (_sharonRoomLightSwitch.IsTurnedOn) _bed.InteractionHitbox.gameObject.SetActive(true);
        else _bed.InteractionHitbox.gameObject.SetActive(false);
    }

    private IEnumerator SleepAnimation()
    {
        InputProvider.Instance.TurnOffPlayerMaps();
        PlayerObjects.Instance.PlayerEquipment.ChangeItem(ItemType.NONE);
        _bed.InteractionHitbox.gameObject.SetActive(false);

        _puttingOffCamera.enabled = true;
        PlayerObjects.Instance.PlayerVirtualCamera.enabled = false;
        yield return null;

        while (CameraManager.Instance.CameraBrain.IsBlending)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        _crutches.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);

        _lyingInBedCamera.enabled = true;
        _puttingOffCamera.enabled = false;
        yield return null;

        while (CameraManager.Instance.CameraBrain.IsBlending)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);

        InputProvider.Instance.TurnOffGameplayOverlayMap();
        Blackout blackoutBackground = Instantiate(_blackoutPrefab);
        blackoutBackground.SetAlphaToZero();

        Tween fadeTween = blackoutBackground.Image.DOFade(1, _fadingTime);
        while (fadeTween.IsActive())
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);

        GameManager.Instance.LoadSceneAndSaveGameState(_nextScene);
    }
}
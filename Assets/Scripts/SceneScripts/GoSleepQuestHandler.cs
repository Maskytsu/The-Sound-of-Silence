using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using NaughtyAttributes;
using UnityEngine.SceneManagement;

public class GoSleepQuestHandler : MonoBehaviour
{
    public Action OnAnimationEnd;

    [Header("Prefabs")]
    [SerializeField] private BlackoutBackground _blackoutBackgroundPrefab;
    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _checkPhoneQuest;
    [SerializeField] private QuestScriptable _goSleepQuest;
    [Header("Scene Objects")]
    [SerializeField] private Bed _bedHitbox;
    [SerializeField] private GameObject _crutches;
    [SerializeField] private GameObject _hearingAid;
    [SerializeField] private CinemachineVirtualCamera _puttingOffCamera;
    [SerializeField] private CinemachineVirtualCamera _lyingInBedCamera;
    [SerializeField] private List<LightSwitch> _lightSwitches;
    [Header("Parameters")]
    [SerializeField] private bool _changeSceneOnEnd = true;
    [ShowIf(nameof(_changeSceneOnEnd))]
    [SerializeField] private string _nextScene;

    private float _fadingTime = 2f;

    private void Start()
    {
        _checkPhoneQuest.OnQuestEnd += () => StartCoroutine(StartSleepQuestDelayed(2f));
        _goSleepQuest.OnQuestStart += StartCheckingAllLights;
        _bedHitbox.OnInteract += () => StartCoroutine(SleepAnimation());
    }

    private IEnumerator StartSleepQuestDelayed(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        QuestManager.Instance.StartQuest(_goSleepQuest);
    }

    private void StartCheckingAllLights()
    {
        CheckAllLights();

        foreach (LightSwitch lightSwtich in _lightSwitches)
        {
            lightSwtich.OnInteract += CheckAllLights;
        }
    }

    private void CheckAllLights()
    {
        bool allLightsOff = true;

        foreach (LightSwitch lightSwtich in _lightSwitches)
        {
            if (lightSwtich.IsTurnedOn)
            {
                allLightsOff = false;
                break;
            }
        }

        if (allLightsOff) _bedHitbox.gameObject.SetActive(true);
        else _bedHitbox.gameObject.SetActive(false);
    }

    private IEnumerator SleepAnimation()
    {
        InputProvider.Instance.TurnOffPlayerMaps();
        yield return StartCoroutine(PlayerManager.Instance.PlayerEquipment.ChangeItem(PlayerEquipment.ItemType.NONE));
        _bedHitbox.gameObject.SetActive(false);

        PlayerManager.Instance.PlayerVisuals.SetActive(false);

        _puttingOffCamera.enabled = true;
        PlayerManager.Instance.PlayerVirtualCamera.enabled = false;
        yield return null;

        while (CameraManager.Instance.CameraBrain.IsBlending)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        _hearingAid.SetActive(true);
        yield return new WaitForSeconds(1f);

        _crutches.SetActive(true);
        yield return new WaitForSeconds(1f);


        _lyingInBedCamera.enabled = true;
        _puttingOffCamera.enabled = false;
        yield return null;

        while (CameraManager.Instance.CameraBrain.IsBlending)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);

        BlackoutBackground blackoutBackground = Instantiate(_blackoutBackgroundPrefab);
        blackoutBackground.StartAlphaFromZero();

        Tween fadeTween = blackoutBackground.Image.DOFade(1, _fadingTime);

        while (fadeTween.IsActive())
        {
            yield return null;
        }
        yield return new WaitForSeconds(1f);

        if (_changeSceneOnEnd) SceneManager.LoadScene(_nextScene);

        OnAnimationEnd?.Invoke();
    }
}

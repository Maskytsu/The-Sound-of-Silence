using Cinemachine;
using System.Collections;
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
    [SerializeField] private QuestScriptable _goSleepQuest;
    [Header("Scene Objects")]
    [SerializeField] private Bed _bed;
    [SerializeField] private Crutches _crutches;
    [SerializeField] private HearingAid _hearingAid;
    [SerializeField] private CinemachineVirtualCamera _puttingOffCamera;
    [SerializeField] private CinemachineVirtualCamera _lyingInBedCamera;
    [Header("Parameters")]
    [SerializeField] private bool _changeSceneOnEnd = true;
    [ShowIf(nameof(_changeSceneOnEnd))]
    [Scene, SerializeField] private string _nextScene;

    private LightSwitch[] _lightSwitches;
    private float _fadingTime = 2f;

    private void Start()
    {
        _goSleepQuest.OnQuestStart += StartCheckingAllLights;
        _bed.OnInteract += () => StartCoroutine(SleepAnimation());

        _lightSwitches = FindObjectsOfType<LightSwitch>();
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

        if (allLightsOff) _bed.InteractionHitbox.gameObject.SetActive(true);
        else _bed.InteractionHitbox.gameObject.SetActive(false);
    }

    private IEnumerator SleepAnimation()
    {
        InputProvider.Instance.TurnOffPlayerMaps();
        yield return StartCoroutine(PlayerManager.Instance.PlayerEquipment.ChangeItem(PlayerEquipment.ItemType.NONE));
        _bed.InteractionHitbox.gameObject.SetActive(false);

        PlayerManager.Instance.PlayerVisuals.SetActive(false);

        _puttingOffCamera.enabled = true;
        PlayerManager.Instance.PlayerVirtualCamera.enabled = false;
        yield return null;

        while (CameraManager.Instance.CameraBrain.IsBlending)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        _hearingAid.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);

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
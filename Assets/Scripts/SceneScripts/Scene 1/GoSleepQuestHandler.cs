using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoSleepQuestHandler : MonoBehaviour
{
    [SerializeField] private QuestScriptable _goSleepQuest;

    [SerializeField] private Bed _bed;
    [SerializeField] private GameObject _bedHitbox;

    [SerializeField] private GameObject _crutches;
    [SerializeField] private GameObject _hearingAid;

    [SerializeField] private CinemachineVirtualCamera _puttingOffCamera;
    [SerializeField] private CinemachineVirtualCamera _lyingInBedCamera;

    [SerializeField] private BlackoutBackground _blackoutBackgroundPrefab;

    [SerializeField] private List<LightSwitch> _lightSwitches;

    private float _fadeSpeed = 2f;
    private BlackoutBackground _blackoutBackground;

    private void Awake()
    {
        _goSleepQuest.OnQuestStart += StartCheckingAllLights;
        _bed.OnInteract += StartSleepAnimation;
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

        if (allLightsOff) _bed.gameObject.SetActive(true);
        else _bed.gameObject.SetActive(false);
    }

    private void StartSleepAnimation()
    {
        StartCoroutine(SleepAnimation());
    }
    private IEnumerator SleepAnimation()
    {
        _bed.gameObject.SetActive(false);
        PlayerManager.Instance.PlayerVisuals.SetActive(false);

        _puttingOffCamera.enabled = true;
        PlayerManager.Instance.VirtualMainCamera.enabled = false;

        yield return null;

        while (PlayerManager.Instance.CameraBrain.IsBlending)
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

        while (PlayerManager.Instance.CameraBrain.IsBlending)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        _blackoutBackground = Instantiate(_blackoutBackgroundPrefab);
        float alpha = 0f;
        RawImage blackoutImage = _blackoutBackground.Image;
        blackoutImage.color = new Color(blackoutImage.color.r, blackoutImage.color.g, blackoutImage.color.b, alpha);

        while (alpha < 1f)
        {
            alpha += Time.deltaTime / _fadeSpeed;
            blackoutImage.color = new Color(blackoutImage.color.r, blackoutImage.color.g, blackoutImage.color.b, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        
        if (GameState.Instance.TookPills) Debug.Log("Change scene to secret ending.");
        else Debug.Log("Change scene to next level.");
    }
}

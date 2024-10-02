using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using UnityEngine.SceneManagement;

public class GoSleepQuestHandler : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] private List<LightSwitch> _lightSwitches;
    [SerializeField] private QuestScriptable _goSleepQuest;
    [Header("Bed Interaction")]
    [SerializeField] private Bed _bed;
    [SerializeField] private GameObject _bedHitbox;
    [Header("Showing Crutches And Hearing Aid")]
    [SerializeField] private GameObject _crutches;
    [SerializeField] private GameObject _hearingAid;
    [Header("Go Sleeping Animation Cameras")]
    [SerializeField] private CinemachineVirtualCamera _puttingOffCamera;
    [SerializeField] private CinemachineVirtualCamera _lyingInBedCamera;
    [Header("Changing Scene")]
    [SerializeField] private BlackoutBackground _blackoutBackgroundPrefab;
    [Scene, SerializeField] private string _scene2;
    [Scene, SerializeField] private string _secretEnding1;


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
        
        if (!GameState.Instance.TookPills) SceneManager.LoadScene(_scene2);
        else SceneManager.LoadScene(_secretEnding1);
    }
}

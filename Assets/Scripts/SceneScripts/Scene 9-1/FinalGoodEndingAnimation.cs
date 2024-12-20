using Cinemachine;
using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class FinalGoodEndingAnimation : MonoBehaviour
{
    [SerializeField] private Blackout _blackout;
    [SerializeField] private TextMeshProUGUI _yearTMP;
    [Space]
    [SerializeField] private CinemachineVirtualCamera _lookAtCamera;
    [SerializeField] private PlayerTargetTransform _ptt;
    [Space]
    [Scene, SerializeField] private string _nextScene;

    private float _fadingSpeed = 1.5f;
    private float _displayTime = 2f;
    private float _walkingDuration = 20f;

    void Start()
    {
        StartCoroutine(Animation());
    }

    private IEnumerator Animation()
    {
        StartCoroutine(PlayerObjects.Instance.PlayerMovement.SetTransformAnimation(_ptt, _walkingDuration));
        float savedTime = Time.time;

        Tween fadingInTMPTween = _yearTMP.DOFade(1f, _fadingSpeed);
        while (fadingInTMPTween.IsActive()) yield return null;

        yield return new WaitForSeconds(_displayTime);
        _blackout.gameObject.SetActive(false);
        InputProvider.Instance.TurnOnGameplayOverlayMap();

        yield return new WaitForSeconds(1f);
        Tween fadingOutTMPTween = _yearTMP.DOFade(0f, _fadingSpeed);

        yield return new WaitForSeconds(2f);
        _lookAtCamera.enabled = true;
        PlayerObjects.Instance.PlayerVirtualCamera.enabled = false;

        yield return new WaitForSeconds(3f);
        PlayerObjects.Instance.PlayerVirtualCamera.enabled = true;
        _lookAtCamera.enabled = false;

        float remainigWalkingTime = _walkingDuration - (Time.time - savedTime);

        if (remainigWalkingTime - 1 > 0) yield return new WaitForSeconds(remainigWalkingTime - 1);
        _blackout.gameObject.SetActive(true);
        InputProvider.Instance.TurnOffGameplayOverlayMap();

        yield return new WaitForSeconds(1.5f);
        GameManager.Instance.LoadSceneAndSaveGameState(_nextScene);
    }
}

using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FullCarDreamAnimation : MonoBehaviour
{
    [Header("Scene Objects")]
    [SerializeField] private EndingDreamTrafficLightHandler _endingHandler;
    [SerializeField] private CarDreamHandler _carHandler;

    private float _closeEyesSpeed = 3.0f;

    private EventInstance _carRadioMusicInstance;
    private EventInstance _carEngineInstance;
    private EventInstance _carCrashInstance;

    private void Start()
    {
        InputProvider.Instance.TurnOnPlayerCameraMap();
        StartCoroutine(CarAnimation());
    }

    private void OnDestroy()
    {
        DOTween.KillAll();
        if (_carRadioMusicInstance.isValid()) _carRadioMusicInstance.Stop();
        if (_carEngineInstance.isValid()) _carEngineInstance.Stop();
        if (_carCrashInstance.isValid()) _carCrashInstance.Stop();
    }

    private IEnumerator CarAnimation()
    {
        StartAudio();
        var blink = HUD.Instance.Blink;
        blink.SetActiveFullBlackout(true);

        yield return new WaitForSeconds(_carHandler.blackoutTime);

        StartCoroutine(_carHandler.OpenEyes(blink));
        _carHandler.TweenPlayerCar(false);
        _carHandler.TweenCrashCar();
        var closeEyesDuration = blink.GetCloseEyesDuration(_closeEyesSpeed);

        yield return new WaitForSeconds(_carHandler.carMovementDurationDream2 - closeEyesDuration - 0.25f);

        InputProvider.Instance.TurnOffGameplayOverlayMap();
        blink.PlayCloseEyes(_closeEyesSpeed);

        yield return new WaitForSeconds(closeEyesDuration);

        _carEngineInstance.Stop();
        _carCrashInstance = RuntimeManager.CreateInstance(FmodEvents.Instance.CarCrash);
        _carCrashInstance.start();
        _carCrashInstance.release();

        yield return new WaitForSeconds(_carRadioMusicInstance.GetRemainingTime() + 0.5f);

        SceneManager.LoadScene(_endingHandler.NextScene);
    }

    private void StartAudio()
    {
        _carRadioMusicInstance = RuntimeManager.CreateInstance(FmodEvents.Instance.SPT_CarRadioSongFull);
        RuntimeManager.AttachInstanceToGameObject(_carRadioMusicInstance, _carHandler.carRadioPosition);
        _carRadioMusicInstance.start();
        _carRadioMusicInstance.release();

        _carEngineInstance = RuntimeManager.CreateInstance(FmodEvents.Instance.CarDreamEngine);
        _carEngineInstance.start();
        _carEngineInstance.release();
    }
}

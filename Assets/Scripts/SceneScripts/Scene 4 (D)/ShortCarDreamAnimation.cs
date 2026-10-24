using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShortCarDreamAnimation : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private Blackout _whiteBlackoutPrefab;
    [Header("Scene Objects")]
    [SerializeField] private CarDreamHandler _carHandler;
    [Header("Parameters")]
    [Scene, SerializeField] private string _nextScene;

    private float _fadingOutTime = 1f;

    private EventInstance _carRadioMusicInstance;
    private EventInstance _carEngineInstance;

    private void Start()
    {
        InputProvider.Instance.TurnOnPlayerCameraMap();
        StartCoroutine(CarAnimation());
    }

    private void OnDestroy()
    {
        if (_carRadioMusicInstance.isValid()) _carRadioMusicInstance.Stop();
        if (_carEngineInstance.isValid()) _carEngineInstance.Stop();
    }

    private IEnumerator CarAnimation()
    {
        StartAudio();
        var blink = HUD.Instance.Blink;
        blink.SetActiveFullBlackout(true);

        yield return new WaitForSeconds(_carHandler.blackoutTime);

        StartCoroutine(_carHandler.OpenEyes(blink));
        _carHandler.TweenPlayerCar(true);

        yield return new WaitForSeconds(_carHandler.carMovementDurationDream1 - _fadingOutTime);

        InputProvider.Instance.TurnOffGameplayOverlayMap();
        Blackout whiteBlackout = Instantiate(_whiteBlackoutPrefab);
        whiteBlackout.SetAlphaToZero();
        Tween fadeTween = whiteBlackout.Image.DOFade(1f, _fadingOutTime);

        while (fadeTween.IsActive()) yield return null;

        _carEngineInstance.Stop();

        yield return new WaitForSeconds(_carHandler.musicEndingTime);
        SceneManager.LoadScene(_nextScene);
    }

    private void StartAudio()
    {
        _carRadioMusicInstance = RuntimeManager.CreateInstance(FmodEvents.Instance.SPT_CarRadioSong);
        RuntimeManager.AttachInstanceToGameObject(_carRadioMusicInstance, _carHandler.carRadioPosition);
        _carRadioMusicInstance.start();
        _carRadioMusicInstance.release();

        _carEngineInstance = RuntimeManager.CreateInstance(FmodEvents.Instance.CarDreamEngine);
        _carEngineInstance.start();
        _carEngineInstance.release();
    }
}

using DG.Tweening;
using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.Video;

public class BlinkEffect : MonoBehaviour
{
    [SerializeField] private VideoClip _blinkOpenEyesClip;
    [SerializeField] private VideoClip _blinkCloseEyesClip;
    [Space]
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private Volume _dofVolume;
    [Space]
    [SerializeField] private RawImage _blackImage;
    [SerializeField] private RawImage _renderImage;
    [Space]
    [Header("Debug")]
    [SerializeField] private float _debugBlinkSpeed = 1.0f;

    private bool _isPrepering = false;
    private bool _isLocked = false;
    private float _dofDuration = 1.0f;

    public bool IsPlaying => _videoPlayer.isPlaying || _isPrepering;

    private void Awake()
    {
        _videoPlayer.sendFrameReadyEvents = true;
        _renderImage.SetAlpha(0.0f);
    }

    public void SetActiveBlackout(bool isActive)
    {
        _dofVolume.weight = isActive ? 1.0f : 0.0f;
        _blackImage.SetAlpha(isActive ? 1.0f : 0.0f);
    }

    public void SetBlinkingLocked(bool isLocked)
    {
        _isLocked = isLocked;
    }

    public float GetCloseEyesDuration(float blinkSpeed = 1.0f)
    {
        return (float)(_blinkCloseEyesClip.length / blinkSpeed);
    }

    public float GetOpenEyesDuration(float blinkSpeed = 1.0f)
    {
        return (float)(_blinkOpenEyesClip.length / blinkSpeed);
    }

    public void PlayCloseEyes(float blinkSpeed)
    {
        if (_isLocked)
        {
            Debug.LogWarning("Tried to blink when it was locked!");
            return;
        }

        if (IsPlaying)
        {
            Debug.LogError("Tried to blink when it was already playing!");
            return;
        }

        _videoPlayer.clip = _blinkCloseEyesClip;
        _videoPlayer.playbackSpeed = blinkSpeed;
        LerpVolume(true, blinkSpeed);

        StartCoroutine(PlayVideo());

        _videoPlayer.loopPointReached += OnCloseEyesFinish;
    }

    public void PlayOpenEyes(float blinkSpeed)
    {
        if (_isLocked)
        {
            Debug.LogWarning("Tried to blink when it was locked!");
            return;
        }

        if (IsPlaying)
        {
            Debug.LogError("Tried to blink when it was already playing!");
            return;
        }

        _videoPlayer.clip = _blinkOpenEyesClip;
        _videoPlayer.playbackSpeed = blinkSpeed;
        LerpVolume(false, blinkSpeed);

        StartCoroutine(PlayVideo(() =>
        {
            _blackImage.SetAlpha(0.0f);
        }));

        _videoPlayer.loopPointReached += OnOpenEyesFinish;
    }

    private void OnCloseEyesFinish(VideoPlayer source)
    {
        _videoPlayer.loopPointReached -= OnCloseEyesFinish;
        _blackImage.SetAlpha(1.0f);
        _renderImage.SetAlpha(0.0f);
    }

    private void OnOpenEyesFinish(VideoPlayer source)
    {
        _videoPlayer.loopPointReached -= OnOpenEyesFinish;
        _renderImage.SetAlpha(0.0f);
    }

    private void LerpVolume(bool toClosed, float blinkSpeed)
    {
        DOTween.To(
            () => _dofVolume.weight,
            x => _dofVolume.weight = x,
            toClosed ? 1.0f : 0.0f,
            _dofDuration / blinkSpeed).SetEase(Ease.InOutSine);
    }

    private IEnumerator PlayVideo(Action afterPrepAction = null)
    {
        _isPrepering = true;

        _videoPlayer.Prepare();

        while (!_videoPlayer.isPrepared) yield return null;
        _videoPlayer.Play();
        // this is to prevent showing render texture with last frame from previous animation
        yield return null;
        yield return null;
        yield return null;

        _renderImage.SetAlpha(1.0f);
        if (afterPrepAction != null) afterPrepAction();

        _isPrepering = false;
    }

    [Button]
    private void DebugPlayOpenEyes()
    {
        PlayOpenEyes(_debugBlinkSpeed);
    }

    [Button]
    private void DebugPlayCloseEyes()
    {
        PlayCloseEyes(_debugBlinkSpeed);
    }
}
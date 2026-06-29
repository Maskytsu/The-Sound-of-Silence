using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class BlinkEffect : MonoBehaviour
{
    [SerializeField] private VideoClip _blinkOpenEyesClip;
    [SerializeField] private VideoClip _blinkCloseEyesClip;
    [Space]
    [SerializeField] private VideoPlayer _videoPlayer;
    [Space]
    [SerializeField] private RawImage _blackImage;
    [SerializeField] private RawImage _renderImage;

    private bool _isPrepering = false;
    private bool _isLocked = false;

    public bool IsPlaying => _videoPlayer.isPlaying || _isPrepering;

    private void Awake()
    {
        _videoPlayer.sendFrameReadyEvents = true;
        _renderImage.SetAlpha(0.0f);
    }

    public void SetActiveBlackout(bool isActive)
    {
        _blackImage.SetAlpha(isActive ? 1.0f : 0.0f);
    }

    public void SetBlinkingLocked(bool isLocked)
    {
        _isLocked = isLocked;
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
}
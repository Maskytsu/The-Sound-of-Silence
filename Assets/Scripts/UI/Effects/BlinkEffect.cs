using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class BlinkEffect : MonoBehaviour
{
    [SerializeField] private VideoClip _blinkFromBlackClip;
    [SerializeField] private VideoClip _blinkToBlackClip;
    [Space]
    [SerializeField] private VideoPlayer _videoPlayer;
    [Space]
    [SerializeField] private RawImage _blackImage;
    [SerializeField] private RawImage _renderImage;

    private bool _isPrepering = false;

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

    public void PlayBlinkToBlack(float blinkSpeed = 3.0f)
    {
        _videoPlayer.clip = _blinkToBlackClip;
        _videoPlayer.playbackSpeed = blinkSpeed;

        StartCoroutine(PlayVideo());

        _videoPlayer.loopPointReached += OnBlinkToBlackFinish;
    }

    public void PlayBlinkFromBlack(float blinkSpeed = 3.0f)
    {
        _videoPlayer.clip = _blinkFromBlackClip;
        _videoPlayer.playbackSpeed = blinkSpeed;

        StartCoroutine(PlayVideo(() =>
        {
            _blackImage.SetAlpha(0.0f);
        }));

        _videoPlayer.loopPointReached += OnBlinkFromBlackFinish;
    }

    private void OnBlinkToBlackFinish(VideoPlayer source)
    {
        _videoPlayer.loopPointReached -= OnBlinkToBlackFinish;
        _blackImage.SetAlpha(1.0f);
        _renderImage.SetAlpha(0.0f);
    }

    private void OnBlinkFromBlackFinish(VideoPlayer source)
    {
        _videoPlayer.loopPointReached -= OnBlinkFromBlackFinish;
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

    /*
    [Button]
    private void SetActiveBlackout()
    {
        _blackImage.SetAlpha(1.0f);
    }

    [SerializeField] private float playbackTime = 1.0f;

    [Button]
    private void PlayBlinkToBlack()
    {
        PlayBlinkToBlack(1.0f);
    }

    [Button]
    private void PlayBlinkFromBlack()
    {
        PlayBlinkFromBlack(1.0f);
    }
    */
}
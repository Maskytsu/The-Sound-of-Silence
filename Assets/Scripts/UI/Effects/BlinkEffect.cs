using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class BlinkEffect : MonoBehaviour
{
    private Action OnPlayerStart;
    private Action OnPlayerFinish;

    [SerializeField] private VideoClip _blinkFromBlackClip;
    [SerializeField] private VideoClip _blinkToBlackClip;
    [Space]
    [SerializeField] private VideoPlayer _videoPlayer;
    [Space]
    [SerializeField] private RawImage _blackImage;
    [SerializeField] private RawImage _renderImage;

    private bool _wasPlaying = false;

    public bool IsPlaying => _videoPlayer.isPlaying;

    private void LateUpdate()
    {
        if (_wasPlaying != _videoPlayer.isPlaying)
        {
            if (_videoPlayer.isPlaying) OnPlayerStart?.Invoke();
            else OnPlayerFinish?.Invoke();
        }

        _wasPlaying = _videoPlayer.isPlaying;
    }

    [Button]
    public void PlayBlinkToBlack()
    {
        _renderImage.SetAlpha(1.0f);
        _videoPlayer.clip = _blinkToBlackClip;
        _videoPlayer.Play();
        OnPlayerFinish += OnBlinkToBlackFinish;
    }

    [Button]
    public void PlayBlinkFromBlack()
    {
        _blackImage.SetAlpha(0.0f);
        _renderImage.SetAlpha(1.0f);
        _videoPlayer.clip = _blinkFromBlackClip;
        _videoPlayer.Play();
        OnPlayerFinish += OnBlinkFromBlackFinish;
    }

    private void OnBlinkToBlackFinish()
    {
        OnPlayerFinish -= OnBlinkToBlackFinish;
        _blackImage.SetAlpha(1.0f);
        _renderImage.SetAlpha(0.0f);
    }

    private void OnBlinkFromBlackFinish()
    {
        OnPlayerFinish -= OnBlinkFromBlackFinish;
        _renderImage.SetAlpha(0.0f);
    }
}

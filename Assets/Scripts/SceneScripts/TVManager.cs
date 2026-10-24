using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TVManager : MonoBehaviour
{
    [SerializeField] private bool _showMonster;
    [Space]
    [SerializeField] private GameObject _tvObject;
    [SerializeField] private CanvasGroup _tvCanvasGroup;
    [SerializeField] private RawImage _tvImage;
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private RenderTexture _tvRenderTexture;
    [Space]
    [SerializeField] private VideoClip _talkShow1Clip;
    [SerializeField] private VideoClip _talkShow2Clip;
    [SerializeField] private Texture2D _monsterTex;

    private OccludedAudioEmitter _tvAmbient;

    public GameObject TVObject => _tvObject;

    private void Awake()
    {
        _tvCanvasGroup.alpha = 0.0f;
    }

    private void Start()
    {
        if (_showMonster)
        {
            _tvAmbient = AudioManager.PlayOneShotOccludedRI(FmodEvents.Instance.OCC_MonsterTVAmbient, _tvObject, true, audioOcclusionWidening: 0.85f);
            _tvCanvasGroup.alpha = 1.0f;
            _tvImage.texture = _monsterTex;
        }
    }

    private void OnDestroy()
    {
        if (_tvAmbient != null) _tvAmbient.EndAudio();
    }

    public void SetupTalkShowPt1()
    {
        _tvCanvasGroup.alpha = 1.0f;
        PlayVideo(_talkShow1Clip);
    }

    public IEnumerator TurnOffTV()
    {
        Tween fadeTVTween = _tvCanvasGroup.DOFade(0, 0.5f);
        while (fadeTVTween.IsActive()) yield return null;
        _videoPlayer.Stop();
    }

    public IEnumerator TurnOnTalkShowPt2()
    {
        PlayVideo(_talkShow2Clip);
        Tween fadeTVTween = _tvCanvasGroup.DOFade(1, 0.5f);
        while (fadeTVTween.IsActive()) yield return null;
    }

    private void PlayVideo(VideoClip videoClip)
    {
        _videoPlayer.Stop();

        _videoPlayer.targetTexture = _tvRenderTexture;
        _tvImage.texture = _tvRenderTexture;

        _videoPlayer.clip = videoClip;
        _videoPlayer.isLooping = true;

        _videoPlayer.prepareCompleted -= OnPrepared;
        _videoPlayer.prepareCompleted += OnPrepared;
        _videoPlayer.Prepare();

        void OnPrepared(VideoPlayer _) => _videoPlayer.Play(); 
    }
}
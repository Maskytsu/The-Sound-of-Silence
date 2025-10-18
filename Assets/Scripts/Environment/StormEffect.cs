using FMODUnity;
using NaughtyAttributes;
using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class StormEffect : MonoBehaviour
{
    public static Color LightningAmbientColor { get; } = new (0.43f, 0.43f, 0.43f);
    
    public event Action OnLightningEnd;

    [SerializeField] private Transform _rainParent;
    [Space]
    [SerializeField] private bool _playLightnings = true;
    [SerializeField] private bool _overrideBaseIntensity = false;
    [FormerlySerializedAs("_baseIntensityValue")] [SerializeField, ShowIf(nameof(_overrideBaseIntensity))] private Color _baseAmbientColor;

    private bool _isEffectPlaying = false;

    private Transform Player => PlayerObjects.Instance.Player.transform;

    private void OnEnable()
    {
        if (!_overrideBaseIntensity) _baseAmbientColor = RenderSettings.ambientLight;

        if (_playLightnings) StartCoroutine(PlayLightningEffects());
    }

    private void Update()
    {
        _rainParent.position = new Vector3(Player.position.x, 0, Player.position.z);
    }

    private void OnDisable()
    {
        StopAllCoroutines();

        RenderSettings.ambientLight = _baseAmbientColor;
        _isEffectPlaying = false;
    }

    public void DeactivateEffect()
    {
        OnLightningEnd -= DeactivateEffect;

        if (!_isEffectPlaying) gameObject.SetActive(false);
        else OnLightningEnd += DeactivateEffect;
    }

    private IEnumerator PlayLightningEffects()
    {
        while (true)
        {
            float delayTime = UnityEngine.Random.Range(10f, 35f);

            yield return new WaitForSeconds(delayTime);

            if (!_isEffectPlaying) StartCoroutine(LightningEffect(0.1f));
        }
    }

    public IEnumerator LightningEffect(float brightTime)
    {
        _isEffectPlaying = true;
        
        RuntimeManager.PlayOneShot(FmodEvents.Instance.Thunder);
        
        float fadeSpeed = 0.25f;
        yield return TweenAmbientLightToColor(LightningAmbientColor, fadeSpeed);
        yield return new WaitForSeconds(brightTime);
        yield return TweenAmbientLightToColor(_baseAmbientColor, fadeSpeed);

        _isEffectPlaying = false;

        //has to be after swaping _isEffectPlaying
        OnLightningEnd?.Invoke();
    }

    public IEnumerator TweenAmbientLightToColor (Color targetColor, float duration)
    {
        Tween fadeAmbientLightColorR = DOTween.To(
            x => RenderSettings.ambientLight = new (x , RenderSettings.ambientLight.g, RenderSettings.ambientLight.b),
            RenderSettings.ambientLight.r,
            targetColor.r,
            duration);
        
        Tween fadeAmbientLightColorG = DOTween.To(
            x => RenderSettings.ambientLight = new (RenderSettings.ambientLight.r, x, RenderSettings.ambientLight.b),
            RenderSettings.ambientLight.g,
            targetColor.g,
            duration);
        
        Tween fadeAmbientLightColorB = DOTween.To(
            x => RenderSettings.ambientLight = new (RenderSettings.ambientLight.r, RenderSettings.ambientLight.g, x),
            RenderSettings.ambientLight.b,
            targetColor.b,
            duration);
        
        while (fadeAmbientLightColorR.IsPlaying() || fadeAmbientLightColorG.IsPlaying() || fadeAmbientLightColorB.IsPlaying())
        {
            yield return null;
        }        
        
        RenderSettings.ambientLight = targetColor;
    }
}
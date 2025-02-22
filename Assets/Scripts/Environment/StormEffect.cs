using FMODUnity;
using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;

public class StormEffect : MonoBehaviour
{
    public event Action OnLightningEnd;

    [SerializeField] private Transform _rainParent;
    [Space]
    [SerializeField] private bool _playLightnings = true;
    [SerializeField, ShowIf(nameof(_playLightnings))] private float _baseIntensityValue = 1f;
    [SerializeField, ShowIf(nameof(_playLightnings))] private float _lightningIntensityValue = 8f;

    private bool _isEffectPlaying = false;

    private Transform Player => PlayerObjects.Instance.Player.transform;

    private void OnEnable()
    {
        if (_playLightnings) StartCoroutine(PlayLightningEffects());
    }

    private void Update()
    {
        _rainParent.position = new Vector3(Player.position.x, 0, Player.position.z);
    }

    private void OnDisable()
    {
        StopAllCoroutines();

        RenderSettings.ambientIntensity = _baseIntensityValue;
        _isEffectPlaying = false;
    }

    private IEnumerator PlayLightningEffects()
    {
        while (true)
        {
            float delayTime = UnityEngine.Random.Range(10f, 30f);

            yield return new WaitForSeconds(delayTime);

            if (!_isEffectPlaying) StartCoroutine(LightningEffect(0.1f));
        }
    }

    public IEnumerator LightningEffect(float brightTime)
    {
        _isEffectPlaying = true;

        //sound
        RuntimeManager.PlayOneShot(FmodEvents.Instance.Thunder);

        //turn intensity up
        float startingIntensityValue = RenderSettings.ambientIntensity;
        float currentIntensityValue = startingIntensityValue;
        float fadeSpeed = 0.25f;
        float distanceBetween = Mathf.Abs(_lightningIntensityValue - startingIntensityValue);

        while (RenderSettings.ambientIntensity < _lightningIntensityValue)
        {
            currentIntensityValue += distanceBetween * (Time.deltaTime / fadeSpeed);
            RenderSettings.ambientIntensity = currentIntensityValue;
            yield return null;
        }
        RenderSettings.ambientIntensity = _lightningIntensityValue;

        //pause at full
        yield return new WaitForSeconds(brightTime);

        //turn intensity down
        currentIntensityValue = _lightningIntensityValue;
        distanceBetween = Mathf.Abs(_lightningIntensityValue - _baseIntensityValue);

        while (RenderSettings.ambientIntensity > _baseIntensityValue)
        {
            currentIntensityValue -= distanceBetween * (Time.deltaTime / fadeSpeed);
            RenderSettings.ambientIntensity = currentIntensityValue;
            yield return null;
        }
        RenderSettings.ambientIntensity = _baseIntensityValue;

        OnLightningEnd?.Invoke();
        _isEffectPlaying = false;
    }
}
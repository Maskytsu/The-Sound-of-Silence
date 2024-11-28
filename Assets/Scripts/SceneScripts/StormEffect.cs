using FMODUnity;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class StormEffect : MonoBehaviour
{
    [SerializeField] private float _baseIntensityValue = 1f;
    [SerializeField] private float _lightningIntensityValue = 8f;
    [SerializeField] private EventReference _thunderSound;

    private bool _isEffectPlaying = false;

    private void Start()
    {
        StartCoroutine(PlayLightningEffects());
    }

    private IEnumerator PlayLightningEffects()
    {
        while (true)
        {
            float delayTime = Random.Range(10f, 30f);

            yield return new WaitForSeconds(delayTime);

            if (!_isEffectPlaying) StartCoroutine(LightningEffect(0.1f));
        }
    }

    public IEnumerator LightningEffect(float brightTime)
    {
        _isEffectPlaying = true;

        //sound
        RuntimeManager.PlayOneShot(_thunderSound);

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

        _isEffectPlaying = false;
    }
}
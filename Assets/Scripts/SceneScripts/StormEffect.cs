using FMODUnity;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class StormEffect : MonoBehaviour
{
    [SerializeField] private EventReference _thunderSound;

    private void Start()
    {
        StartCoroutine(PlayLightningEffects());
    }

    private IEnumerator PlayLightningEffects()
    {
        yield return null;

        while (true)
        {
            float delayTime = Random.Range(10f, 30f);

            yield return new WaitForSeconds(delayTime);

            StartCoroutine(LightningEffect());
        }
    }

    private IEnumerator LightningEffect()
    {
        RuntimeManager.PlayOneShot(_thunderSound);

        float startingIntensityValue = RenderSettings.ambientIntensity;
        float intensityValue = startingIntensityValue;
        float targetIntensity = 8f;
        float fadeSpeed = 0.25f;
        float distanceBetween = Mathf.Abs(targetIntensity - startingIntensityValue);

        while (RenderSettings.ambientIntensity < targetIntensity)
        {
            intensityValue += distanceBetween * (Time.deltaTime / fadeSpeed);
            RenderSettings.ambientIntensity = intensityValue;
            yield return null;
        }
        RenderSettings.ambientIntensity = targetIntensity;
        intensityValue = targetIntensity;


        yield return new WaitForSeconds(0.1f);


        while (RenderSettings.ambientIntensity > startingIntensityValue)
        {
            intensityValue -= distanceBetween * (Time.deltaTime / fadeSpeed);
            RenderSettings.ambientIntensity = intensityValue;
            yield return null;
        }
        RenderSettings.ambientIntensity = startingIntensityValue;
    }

    //---------------------------------------------------------
    [Button]
    private void PlayLightning()
    {
        StartCoroutine(LightningEffect());
    }
}
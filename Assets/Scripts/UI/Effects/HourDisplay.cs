using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HourDisplay : MonoBehaviour
{
    public string HourText;
    public Action OnSelfDestroy;

    [SerializeField] private TextMeshProUGUI _hourTMP;
    [SerializeField] private RawImage _background;

    private float _fadeSpeed = 1.5f;

    private IEnumerator Start()
    {
        InputProvider.Instance.TurnOffPlayerMaps();
        _hourTMP.text = HourText;

        float alpha = 0f;

        _hourTMP.color = new Color(_hourTMP.color.r, _hourTMP.color.g, _hourTMP.color.b, alpha);

        while (alpha < 1)
        {
            alpha += Time.deltaTime / _fadeSpeed;
            _hourTMP.color = new Color(_hourTMP.color.r, _hourTMP.color.g, _hourTMP.color.b, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        while (alpha > 0)
        {
            alpha -= Time.deltaTime / _fadeSpeed;
            _hourTMP.color = new Color(_hourTMP.color.r, _hourTMP.color.g, _hourTMP.color.b, alpha);
            _background.color = new Color(_background.color.r, _background.color.g, _background.color.b, alpha);
            yield return null;
        }

        //InputProvider.Instance.TurnOnPlayerMaps();

        OnSelfDestroy?.Invoke();
        Destroy(gameObject);
    }
}

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

    private IEnumerator Start()
    {
        InputProvider.Instance.TurnOffPlayerMap();
        _hourTMP.text = HourText;

        float alpha = 0;
        float textSpeed = 3f;

        _hourTMP.color = new Color(_hourTMP.color.r, _hourTMP.color.g, _hourTMP.color.b, alpha);

        while (alpha < 1)
        {
            alpha += Time.deltaTime / textSpeed;
            _hourTMP.color = new Color(_hourTMP.color.r, _hourTMP.color.g, _hourTMP.color.b, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(1);

        while (alpha > 0)
        {
            alpha -= Time.deltaTime / textSpeed;
            _hourTMP.color = new Color(_hourTMP.color.r, _hourTMP.color.g, _hourTMP.color.b, alpha);
            _background.color = new Color(_background.color.r, _background.color.g, _background.color.b, alpha);
            yield return null;
        }

        InputProvider.Instance.TurnOnPlayerMap();

        OnSelfDestroy?.Invoke();
        Destroy(gameObject);
    }
}

using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HourDisplay : MonoBehaviour
{
    public string HourText;
    public event Action OnSelfDestroy;

    [SerializeField] private TextMeshProUGUI _hourTMP;
    [SerializeField] private BlackoutBackground _blackoutBackgroundPrefab;

    private BlackoutBackground _blackoutBackground;
    private float _fadeSpeed = 1.5f;

    private IEnumerator Start()
    {
        InputProvider.Instance.TurnOffPlayerMaps();
        _hourTMP.text = HourText;

        _blackoutBackground = Instantiate(_blackoutBackgroundPrefab);

        float alpha = 0f;

        _hourTMP.color = new Color(_hourTMP.color.r, _hourTMP.color.g, _hourTMP.color.b, alpha);

        while (alpha < 1)
        {
            alpha += Time.deltaTime / _fadeSpeed;
            _hourTMP.color = new Color(_hourTMP.color.r, _hourTMP.color.g, _hourTMP.color.b, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        RawImage blackoutImage = _blackoutBackground.Image;

        while (alpha > 0)
        {
            alpha -= Time.deltaTime / _fadeSpeed;
            _hourTMP.color = new Color(_hourTMP.color.r, _hourTMP.color.g, _hourTMP.color.b, alpha);
            blackoutImage.color = new Color(blackoutImage.color.r, blackoutImage.color.g, blackoutImage.color.b, alpha);
            yield return null;
        }

        yield return null;

        //InputProvider.Instance.TurnOnPlayerMaps();

        OnSelfDestroy?.Invoke();
        Destroy(_blackoutBackground.gameObject);
        Destroy(gameObject);
    }
}

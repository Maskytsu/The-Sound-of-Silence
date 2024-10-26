using DG.Tweening;
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

    private float _fadingSpeed = 1.5f;

    private IEnumerator Start()
    {
        InputProvider.Instance.TurnOffPlayerMaps();
        BlackoutBackground _blackoutBackground = Instantiate(_blackoutBackgroundPrefab);

        _hourTMP.text = HourText;
        _hourTMP.color = new Color(_hourTMP.color.r, _hourTMP.color.g, _hourTMP.color.b, 0f);

        Tween fadingInTMPTween = _hourTMP.DOFade(1f, _fadingSpeed);

        while (fadingInTMPTween.IsActive())
        {
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);

        Tween fadingOutTMPTween = _hourTMP.DOFade(0f, _fadingSpeed);
        Tween fadingOutImageTween = _blackoutBackground.Image.DOFade(1f, _fadingSpeed);

        while (fadingOutTMPTween.IsActive() || fadingOutImageTween.IsActive())
        {
            yield return null;
        }

        //InputProvider.Instance.TurnOnPlayerMaps();

        OnSelfDestroy?.Invoke();
        Destroy(_blackoutBackground.gameObject);
        Destroy(gameObject);
    }
}

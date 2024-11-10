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
    [SerializeField] private Blackout _blackoutPrefab;

    private float _fadingSpeed = 1.5f;

    private IEnumerator Start()
    {
        InputProvider.Instance.TurnOffPlayerMaps();
        Blackout blackoutBackground = Instantiate(_blackoutPrefab);

        _hourTMP.text = HourText;
        _hourTMP.color = new Color(_hourTMP.color.r, _hourTMP.color.g, _hourTMP.color.b, 0f);

        Tween fadingInTMPTween = _hourTMP.DOFade(1f, _fadingSpeed);

        while (fadingInTMPTween.IsActive())
        {
            yield return null;
        }

        yield return new WaitForSeconds(1.5f);

        Sequence sequence = DOTween.Sequence();

        sequence.Append(_hourTMP.DOFade(0f, _fadingSpeed));
        sequence.AppendInterval(0.1f);
        sequence.Append(blackoutBackground.Image.DOFade(0f, _fadingSpeed));

        while (sequence.IsPlaying())
        {
            yield return null;
        }

        InputProvider.Instance.TurnOnPlayerMaps();
        OnSelfDestroy?.Invoke();
        Destroy(blackoutBackground.gameObject);
        Destroy(gameObject);
    }
}

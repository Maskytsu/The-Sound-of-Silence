using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HourDisplay : MonoBehaviour
{
    public string HourText;
    public Action OnSelfDestroy;

    [SerializeField] protected TextMeshProUGUI _hourTMP;
    [SerializeField] protected Blackout _blackoutPrefab;
    [SerializeField] protected float _fadingSpeed = 1.5f;
    [SerializeField] protected float _displayTime = 1.5f;

    private void Start()
    {
        StartCoroutine(DisplayGivenHour());
    }

    protected virtual IEnumerator DisplayGivenHour()
    {
        InputProvider.Instance.TurnOffPlayerMaps();
        Blackout blackout = Instantiate(_blackoutPrefab);

        _hourTMP.text = HourText;

        Tween fadingInTMPTween = _hourTMP.DOFade(1f, _fadingSpeed);
        while (fadingInTMPTween.IsActive())
        {
            yield return null;
        }

        yield return new WaitForSeconds(_displayTime);

        Sequence sequence = DOTween.Sequence();

        sequence.Append(_hourTMP.DOFade(0f, _fadingSpeed));
        sequence.AppendInterval(0.1f);
        sequence.Append(blackout.Image.DOFade(0f, _fadingSpeed));

        while (sequence.IsPlaying())
        {
            yield return null;
        }

        InputProvider.Instance.TurnOnPlayerMaps();

        OnSelfDestroy?.Invoke();
        Destroy(blackout.gameObject);
        Destroy(gameObject);
    }
}

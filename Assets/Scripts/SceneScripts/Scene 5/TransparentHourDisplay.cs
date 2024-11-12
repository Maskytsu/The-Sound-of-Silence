using DG.Tweening;
using System.Collections;
using UnityEngine;

public class TransparentHourDisplay : HourDisplay
{
    protected override IEnumerator DisplayGivenHour()
    {
        _hourTMP.text = HourText;

        Tween fadingInTMPTween = _hourTMP.DOFade(1f, _fadingSpeed);
        while (fadingInTMPTween.IsActive())
        {
            yield return null;
        }

        yield return new WaitForSeconds(_displayTime);

        Tween fadingOutTMPTween = _hourTMP.DOFade(0f, _fadingSpeed);
        while (fadingOutTMPTween.IsActive())
        {
            yield return null;
        }

        OnSelfDestroy?.Invoke();
        Destroy(gameObject);
    }
}

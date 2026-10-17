using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using FMODUnity;

public class ChoiceEndingPanel : MonoBehaviour
{
    [SerializedDictionary("Text", "Display time")]
    [SerializeField] private List<TextMeshProUGUI> _texts;
    [SerializeField] private EndingPanel _endingPanel;

    private float _fadingSpeed = 1.5f;

    private void Start()
    {
        StartCoroutine(TextAnimation());
    }

    private IEnumerator TextAnimation()
    {
        yield return new WaitForSeconds(1f);

        foreach(TextMeshProUGUI tmp in _texts)
        {
            RuntimeManager.PlayOneShot(FmodEvents.Instance.UIEndingDescriptionShown);
            Tween fadingInTMPTween = tmp.DOFade(1f, _fadingSpeed);
            while (fadingInTMPTween.IsActive()) yield return null;

            yield return new WaitForSeconds(tmp.text.GetReadingTime(false) + 1f);
        }

        yield return new WaitForSeconds(1f);

        foreach(TextMeshProUGUI tmp in _texts)
        {
            tmp.DOFade(0f, _fadingSpeed);
        }
        yield return new WaitForSeconds(_fadingSpeed);

        _endingPanel.gameObject.SetActive(true);
    }
}

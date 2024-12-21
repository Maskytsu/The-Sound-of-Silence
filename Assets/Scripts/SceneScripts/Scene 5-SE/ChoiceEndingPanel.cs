using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class ChoiceEndingPanel : MonoBehaviour
{
    [SerializedDictionary("Text", "Display time")]
    [SerializeField] private SerializedDictionary<TextMeshProUGUI, float> _texts;
    [SerializeField] private EndingPanel _endingPanel;

    private float _fadingSpeed = 1.5f;

    private void Start()
    {
        StartCoroutine(TextAnimation());
    }

    private IEnumerator TextAnimation()
    {
        yield return new WaitForSeconds(1f);

        foreach(TextMeshProUGUI tmp in _texts.Keys)
        {
            Tween fadingInTMPTween = tmp.DOFade(1f, _fadingSpeed);
            while (fadingInTMPTween.IsActive()) yield return null;

            yield return new WaitForSeconds(_texts[tmp]);
        }

        yield return new WaitForSeconds(1f);

        foreach(TextMeshProUGUI tmp in _texts.Keys)
        {
            tmp.DOFade(0f, _fadingSpeed);
        }
        yield return new WaitForSeconds(_fadingSpeed);

        _endingPanel.gameObject.SetActive(true);
    }
}

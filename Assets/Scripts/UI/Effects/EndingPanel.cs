using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingPanel : MonoBehaviour
{
    [ReadOnly, SerializeField] private string _firstPart = "Ending number #";
    [SerializeField] private int _endingNumber;
    [ReadOnly, SerializeField] private string _secondPart = " out of 5";
    [SerializeField] private TextMeshProUGUI _endingInfoTMP;
    [Scene, SerializeField] private string _menuScene;

    private float _fadingSpeed = 1.5f;
    private float _displayTime = 1.5f;

    private void Start()
    {
        _endingInfoTMP.text = _firstPart + _endingNumber + _secondPart;
        StartCoroutine(DisplayInfo());
    }

    private IEnumerator DisplayInfo()
    {
        yield return new WaitForSeconds(0.5f);

        Tween fadingInTMPTween = _endingInfoTMP.DOFade(1f, _fadingSpeed);
        while (fadingInTMPTween.IsActive()) yield return null;

        yield return new WaitForSeconds(_displayTime);

        Tween fadingOutTMPTween = _endingInfoTMP.DOFade(0f, _fadingSpeed);
        while (fadingOutTMPTween.IsActive()) yield return null;

        yield return new WaitForSeconds(0.5f);

        SaveManager.Instance.ClearSave();
        SceneManager.LoadScene(_menuScene);
    }
}

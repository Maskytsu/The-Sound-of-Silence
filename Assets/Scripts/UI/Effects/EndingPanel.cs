using DG.Tweening;
using NaughtyAttributes;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingPanel : MonoBehaviour
{
    [ReadOnly, SerializeField] private string _firstPart = "Ending number #";
    [SerializeField] private int _endingNumber;
    [ReadOnly, SerializeField] private string _secondPart = " out of 5";
    [SerializeField] private TextMeshProUGUI _endingInfoTMP;
    [Scene, SerializeField, HideIf(nameof(_isEnding3))] private string _menuScene;
    [Scene, SerializeField, ShowIf(nameof(_isEnding3))] private string _scene1;
    [SerializeField, ShowIf(nameof(_isEnding3))] private string _ending3Text;
    [SerializeField, ShowIf(nameof(_isEnding3))] private DontDestroyOnLoadChecker _checkerPrefab;
    [Space]
    [SerializeField] private bool _isEnding3;

    private float _fadingSpeed = 1.5f;
    private float _displayTime = 1.5f;

    private void Start()
    {
        StartCoroutine(DisplayInfo());
    }

    private IEnumerator DisplayInfo()
    {
        _endingInfoTMP.text = _firstPart + _endingNumber + _secondPart;

        yield return new WaitForSeconds(1f);

        Tween fadingInTMPTween = _endingInfoTMP.DOFade(1f, _fadingSpeed);
        while (fadingInTMPTween.IsActive()) yield return null;

        yield return new WaitForSeconds(_displayTime);

        Tween fadingOutTMPTween = _endingInfoTMP.DOFade(0f, _fadingSpeed);
        while (fadingOutTMPTween.IsActive()) yield return null;

        yield return new WaitForSeconds(1f);

        if (_isEnding3)
        {
            _endingInfoTMP.text = _ending3Text;

            fadingInTMPTween = _endingInfoTMP.DOFade(1f, _fadingSpeed);
            while (fadingInTMPTween.IsActive()) yield return null;

            yield return new WaitForSeconds(_displayTime);

            fadingOutTMPTween = _endingInfoTMP.DOFade(0f, _fadingSpeed);
            while (fadingOutTMPTween.IsActive()) yield return null;

            yield return new WaitForSeconds(1f);

            Instantiate(_checkerPrefab);
        }

        var nextScene = _isEnding3 ? _scene1 : _menuScene;
        SaveManager.Instance.ClearSave();
        SceneManager.LoadScene(nextScene);
    }
}
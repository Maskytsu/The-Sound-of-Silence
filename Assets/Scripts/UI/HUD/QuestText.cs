using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class QuestText : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI TMP;
    [SerializeField] private CanvasGroup _group;

    private float _fadeDuration = 0.4f;

    private void Start()
    {
        _group.alpha = 0.0f;
        StartCoroutine(DisplayQuest());
    }

    private void OnDestroy()
    {
        DOTween.KillAll();
        StopAllCoroutines();
    }

    public virtual IEnumerator DisplayQuest()
    {
        yield return _group.DOFade(1.0f, _fadeDuration).SetDelay(_fadeDuration + 0.1f).WaitForCompletion();
    }

    public IEnumerator DestroyQuestText()
    {
        DOTween.KillAll();
        StopAllCoroutines();

        yield return _group.DOFade(0.0f, _fadeDuration).WaitForCompletion();

        Destroy(gameObject);
    }
}
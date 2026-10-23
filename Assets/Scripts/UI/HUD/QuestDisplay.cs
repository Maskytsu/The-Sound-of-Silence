using DG.Tweening;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class QuestDisplay : MonoBehaviour
{
    [SerializeField] private Transform _questsLayout;
    [SerializeField] private QuestText _questTextPrefab;
    [Space]
    [SerializeField] private CanvasGroup _hiddenQuestsGroup;
    [SerializeField] private TextMeshProUGUI _hiddenTextTMP;
    [Space]
    [SerializeField] float minSpareTime = 8f;
    [SerializeField] float maxSpareTime = 24f;
    [SerializeField] float minDisplayTime = 2f;
    [SerializeField] float maxDisplayTime = 5f;

    private float _fadeDuration = 0.4f;
    private Coroutine _hiddenQuestLoopAnimation;
    private bool _hiddenQuestShown;

    private Dictionary<QuestScriptable, QuestText> _displayedQuests = new ();
    private Dictionary<QuestScriptable, List<string>> _displayedHiddenQuests = new ();

    private List<string> AllHiddenQuestStrings => _displayedHiddenQuests.SelectMany(pair => pair.Value).ToList();

    private void Start()
    {
        _hiddenQuestsGroup.alpha = 0.0f;
        _hiddenTextTMP.color = UIColors.Instance.HiddenQuestText;
    }

    private void OnDestroy()
    {
        DOTween.KillAll();
        StopAllCoroutines();
    }

    //-----------------------------------------

    public void DisplayNewQuest(QuestScriptable quest)
    {
        if (!quest.IsHidden) StartCoroutine(DisplayRegularQuest(quest));
        else DisplayHiddenQuest(quest);
    }

    //-----------------------------------------
    private IEnumerator DisplayRegularQuest(QuestScriptable quest)
    {
        yield return new WaitUntil(() => !_hiddenQuestShown);

        quest.OnQuestEnd += () => RemoveQuestFromDisplay(quest);

        QuestText text = Instantiate(_questTextPrefab, _questsLayout);
        _displayedQuests.Add(quest, text);
        text.TMP.text = quest.QuestName;

        RefreshHiddenQuestPosition();
    }

    public void DisplayHiddenQuest(QuestScriptable quest)
    {
        quest.OnQuestEnd += () => RemoveHiddenQuestFromDisplay(quest);

        _displayedHiddenQuests.Add(quest, quest.QuestTexts);

        if (_hiddenQuestLoopAnimation == null)
        {
            DOTween.Kill(_hiddenQuestsGroup);
            _hiddenQuestLoopAnimation = StartCoroutine(HiddenQuestAnimation());
        }

        RefreshHiddenQuestPosition();
    }

    //-----------------------------------------

    private void RemoveQuestFromDisplay(QuestScriptable quest)
    {
        StartCoroutine(_displayedQuests[quest].DestroyQuestText());
        _displayedQuests.Remove(quest);
    }

    private void RemoveHiddenQuestFromDisplay(QuestScriptable quest)
    {
        _displayedHiddenQuests.Remove(quest);

        if (_displayedHiddenQuests.Count == 0)
        {
            if (_hiddenQuestLoopAnimation != null)
            {
                StopCoroutine(_hiddenQuestLoopAnimation);
                _hiddenQuestLoopAnimation = null;
            }

            DOTween.Kill(_hiddenQuestsGroup);
            _hiddenQuestsGroup.DOFade(0.0f, _fadeDuration);
            _hiddenQuestShown = false;
        }
    }

    //-----------------------------------------

    private IEnumerator HiddenQuestAnimation()
    {
        while (true)
        {
            float spareTime1 = Random.Range(minSpareTime, maxSpareTime);
            float displayTime = Random.Range(minDisplayTime, maxDisplayTime);
            float spareTime2 = Random.Range(minSpareTime, maxSpareTime);

            yield return new WaitForSeconds(spareTime1);

            int randomIndex = Random.Range(0, AllHiddenQuestStrings.Count);
            _hiddenTextTMP.text = AllHiddenQuestStrings[randomIndex];

            _hiddenQuestShown = true;
            yield return _hiddenQuestsGroup.DOFade(1.0f, _fadeDuration).WaitForCompletion();
            yield return new WaitForSeconds(displayTime);
            yield return _hiddenQuestsGroup.DOFade(0.0f, _fadeDuration).WaitForCompletion();
            _hiddenQuestShown = false;

            yield return new WaitForSeconds(spareTime2);
        }
    }

    private void RefreshHiddenQuestPosition()
    {
        _hiddenTextTMP.transform.SetAsLastSibling();
    }
}

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

    private float _fadeDuration = 0.4f;
    private Coroutine _hiddenQuestLoopAnimation;

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
        if (!quest.IsHidden) DisplayRegularQuest(quest);
        else DisplayHiddenQuest(quest);
    }

    //-----------------------------------------

    public void DisplayRegularQuest(QuestScriptable quest)
    {
        quest.OnQuestEnd += () => RemoveQuestFromDisplay(quest);

        QuestText text = Instantiate(_questTextPrefab, _questsLayout);
        _displayedQuests.Add(quest, text);
        text.TMP.text = quest.QuestName;
        RuntimeManager.PlayOneShot(FmodEvents.Instance.NewQuest);
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
        }
    }

    //-----------------------------------------

    private IEnumerator HiddenQuestAnimation()
    {
        while (true)
        {
            //float spareTime1 = Random.Range(10f, 30f);
            float spareTime1 = Random.Range(3f, 3f);
            float displayTime = Random.Range(2f, 5f);
            //float spareTime2 = Random.Range(10f, 30f);
            float spareTime2 = Random.Range(3f, 3f);

            yield return new WaitForSeconds(spareTime1);

            RuntimeManager.PlayOneShot(FmodEvents.Instance.HiddenQuestAppeared);
            int randomIndex = Random.Range(0, AllHiddenQuestStrings.Count);
            _hiddenTextTMP.text = AllHiddenQuestStrings[randomIndex];

            yield return _hiddenQuestsGroup.DOFade(1.0f, _fadeDuration).WaitForCompletion();
            yield return new WaitForSeconds(displayTime);
            yield return _hiddenQuestsGroup.DOFade(0.0f, _fadeDuration).WaitForCompletion();

            yield return new WaitForSeconds(spareTime2);
        }
    }
}

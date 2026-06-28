using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDisplay : MonoBehaviour
{
    [SerializeField] private Transform _questsLayout;
    [SerializeField] private Transform _hiddenQuestLayout;
    [Space]
    [SerializeField] private QuestText _questTextPrefab;
    [SerializeField] private HiddenQuestText _hiddenQuestTextPrefab;

    private Dictionary<QuestScriptable, QuestText> DisplayedQuests = new Dictionary<QuestScriptable, QuestText>();

    public void DisplayNewQuest(QuestScriptable quest)
    {
        if (quest.IsHidden) DisplayHiddenQuest(quest);
        else DisplayRegularQuest(quest);
    }

    public void DisplayRegularQuest(QuestScriptable quest)
    {
        quest.OnQuestEnd += () => RemoveQuestFromDisplay(quest);

        QuestText text = Instantiate(_questTextPrefab, _questsLayout);
        DisplayedQuests.Add(quest, text);
        text.TMP.text = quest.QuestName;
        RuntimeManager.PlayOneShot(FmodEvents.Instance.NewQuest);

        _questsLayout.gameObject.SetActive(true);
    }

    public void DisplayHiddenQuest(QuestScriptable quest)
    {
        quest.OnQuestEnd += () => RemoveQuestFromDisplay(quest);

        HiddenQuestText text = Instantiate(_hiddenQuestTextPrefab, _hiddenQuestLayout);
        text.GetComponent<HiddenQuestText>().Quest = quest;
        DisplayedQuests.Add(quest, text);

        _hiddenQuestLayout.gameObject.SetActive(true);
    }

    private void RemoveQuestFromDisplay(QuestScriptable quest)
    {
        DisplayedQuests[quest].DestroyQuestText();
        Destroy(DisplayedQuests[quest].gameObject);
        DisplayedQuests.Remove(quest);

        //Destory must be executed before it to work properly so it waits for end of frame to execute this after destroy
        StartCoroutine(CheckLayoutsDelayed());
    }

    private IEnumerator CheckLayoutsDelayed()
    {
        yield return new WaitForEndOfFrame();

        if (_questsLayout.childCount == 0) _questsLayout.gameObject.SetActive(false);
        if (_hiddenQuestLayout.childCount == 0) _hiddenQuestLayout.gameObject.SetActive(false);
    }
}

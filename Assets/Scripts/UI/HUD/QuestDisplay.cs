using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestDisplay : MonoBehaviour
{
    [SerializeField] private Transform _questsLayout;
    [SerializeField] private Transform _hiddenQuestLayout;
    [SerializeField] private TextMeshProUGUI _questTextPrefab;
    [SerializeField] private TextMeshProUGUI _hiddenQuestTextPrefab;

    private Dictionary<QuestScriptable, TextMeshProUGUI> DisplayedQuests = new Dictionary<QuestScriptable, TextMeshProUGUI>();

    public void DisplayNewQuest(QuestScriptable quest)
    {
        if (quest.IsHidden) DisplayHiddenQuest(quest);
        else DisplayRegularQuest(quest);
    }

    public void DisplayRegularQuest(QuestScriptable quest)
    {
        quest.OnQuestEnd += () => RemoveQuestFromDisplay(quest);

        TextMeshProUGUI text = Instantiate(_questTextPrefab, _questsLayout);
        DisplayedQuests.Add(quest, text);
        text.text = quest.QuestName;
        RuntimeManager.PlayOneShot(FmodEvents.Instance.NewQuest);

        _questsLayout.gameObject.SetActive(true);
    }

    public void DisplayHiddenQuest(QuestScriptable quest)
    {
        quest.OnQuestEnd += () => RemoveQuestFromDisplay(quest);

        TextMeshProUGUI text = Instantiate(_hiddenQuestTextPrefab, _hiddenQuestLayout);
        text.GetComponent<HiddenQuestText>().Quest = quest;
        DisplayedQuests.Add(quest, text);

        _hiddenQuestLayout.gameObject.SetActive(true);
    }

    private void RemoveQuestFromDisplay(QuestScriptable quest)
    {
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

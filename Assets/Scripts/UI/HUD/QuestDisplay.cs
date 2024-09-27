using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestDisplay : MonoBehaviour
{
    [SerializeField] private Transform _questsLayout;
    [SerializeField] private TextMeshProUGUI _questTextPrefab;

    private Dictionary<QuestScriptable, TextMeshProUGUI> DisplayedQuests = new Dictionary<QuestScriptable, TextMeshProUGUI>();

    public void DisplayNewQuest(QuestScriptable quest)
    {
        quest.OnQuestEnd += RemoveQuest;
        TextMeshProUGUI text = Instantiate(_questTextPrefab, _questsLayout);
        DisplayedQuests.Add(quest, text);
        text.text = quest.QuestName;
    }

    private void RemoveQuest(QuestScriptable quest)
    {
        Destroy(DisplayedQuests[quest].gameObject);
        DisplayedQuests.Remove(quest);
    }
}

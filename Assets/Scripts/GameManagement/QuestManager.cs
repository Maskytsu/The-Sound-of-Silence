using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class QuestManager : MonoBehaviour
{
    [SerializeField] QuestDisplay _questDisplay;

    public static QuestManager Instance { get; private set; }

    public List<QuestScriptable> CurrentQuests = new List<QuestScriptable>();

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one QuestManager in the scene.");
        }
        Instance = this;
    }

    public void StartNewQuest(QuestScriptable quest)
    {
        _questDisplay.DisplayNewQuest(quest);
        quest.OnQuestEnded += QuestManager_OnQuestEnded;
    }

    private void QuestManager_OnQuestEnded(QuestScriptable quest)
    {
        CurrentQuests.Remove(quest);
    }
}

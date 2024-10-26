using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "ScriptableObjects/Quest")]
public class QuestScriptable : ScriptableObject
{
    public string QuestName;
    public bool HiddenQuest;

    public event Action OnQuestStart;
    public event Action<QuestScriptable> OnQuestEnd;
    public event Action<QuestScriptable> OnQuestBreak;

    public void StartQuest()
    {
        Debug.Log(QuestName + " quest started.");
        OnQuestStart?.Invoke();
    }

    public void EndQuest()
    {
        Debug.Log(QuestName + " quest ended.");
        OnQuestEnd?.Invoke(this);
    }

    public void BreakQuest()
    {
        Debug.Log(QuestName + " quest broken.");
        OnQuestBreak?.Invoke(this);
    }

    public void ClearSubscribers()
    {
        OnQuestStart = null;
        OnQuestBreak = null;
        OnQuestEnd = null;
    }
}
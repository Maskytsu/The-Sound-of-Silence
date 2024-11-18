using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "ScriptableObjects/Quest")]
public class QuestScriptable : ScriptableObject
{
    public string QuestName;
    public bool IsHidden;

    public Action OnQuestStart;
    public Action OnQuestEnd;

    public void ClearSubscribers()
    {
        OnQuestStart = null;
        OnQuestEnd = null;
    }
}
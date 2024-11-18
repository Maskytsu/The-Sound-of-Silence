using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Quest", menuName = "ScriptableObjects/Quest")]
public class QuestScriptable : ScriptableObject
{
    public bool IsHidden;
    [HideIf(nameof(IsHidden))] public string QuestName;
    [ShowIf(nameof(IsHidden))] public List<string> QuestTexts;

    public Action OnQuestStart;
    public Action OnQuestEnd;

    public void ClearSubscribers()
    {
        OnQuestStart = null;
        OnQuestEnd = null;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestScriptable", menuName = "ScriptableObjects/QuestScriptable")]
public class QuestScriptable : ScriptableObject
{
    public string QuestName;
    public bool HiddenQuest;

    public Action<QuestScriptable> OnQuestEnded;

    public void EndQuest()
    {
        OnQuestEnded?.Invoke(this);
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestScriptable", menuName = "ScriptableObjects/QuestScriptable")]
public class QuestScriptable : ScriptableObject
{
    public string QuestName;
    public bool HiddenQuest;

    public Action<QuestScriptable> OnQuestEnd;

    public void EndQuest()
    {
        Debug.Log(QuestName + " quest ended");
        OnQuestEnd?.Invoke(this);
    }
}
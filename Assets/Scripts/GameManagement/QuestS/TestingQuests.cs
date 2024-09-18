using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingQuests : Interactable
{
    [SerializeField] private QuestScriptable _quest;

    private void Start()
    {
        QuestManager.Instance.StartNewQuest(_quest);
    }
    
    public override void Interact()
    {
        _quest.EndQuest();
        Destroy(transform.parent.gameObject);
    }
}

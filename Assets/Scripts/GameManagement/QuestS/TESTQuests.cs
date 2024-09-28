using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTQuests : Interactable
{
    [SerializeField] private QuestScriptable _quest;

    private void Start()
    {
        UIDisplayManager.Instance.DisplayNewQuest(_quest);
    }
    
    public override void Interact()
    {
        _quest.EndQuest();
        Destroy(transform.parent.gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CouchEndingHandler : MonoBehaviour
{
    [Header("Scriptable Objects")]
    [SerializeField] private QuestScriptable _couchQuest;
    //[Header("Scene Objects")]

    private void Start()
    {
        _couchQuest.OnQuestStart += BeginCouchQuest;
    }

    private void BeginCouchQuest()
    {

    }
}

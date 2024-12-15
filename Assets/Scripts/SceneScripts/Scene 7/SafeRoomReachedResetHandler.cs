using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeRoomReachedResetHandler : MonoBehaviour
{
    [Header("If reached")]
    [SerializeField] private QuestScriptable _escapeQuest;
    [SerializeField] private QuestScriptable _resetBreakers;
    [SerializeField] private List<GameObject> _objToTurnOn;
    [SerializeField] private List<GameObject> _objToTurnOff;
    [Header("If took gun")]
    [SerializeField] private QuestScriptable _killQuest;

    //needs to work on Awake from Scene7ResetHandler
    public void PrepareScene()
    {
        SetActiveObjects(_objToTurnOn, true);
        SetActiveObjects(_objToTurnOff, false);
    }

    private void SetActiveObjects(List<GameObject> gObjects, bool activeStateToSet)
    {
        foreach (GameObject gObject in gObjects)
        {
            gObject.SetActive(activeStateToSet);
        }
    }
}

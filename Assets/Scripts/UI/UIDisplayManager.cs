using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDisplayManager : MonoBehaviour
{
    public static UIDisplayManager Instance { get; private set; }

    public HUD HUD;
    [HideInInspector] public List<QuestScriptable> CurrentQuests = new List<QuestScriptable>();
    public Action OnHourDisplayEnd;

    [Header("HourDisplay")]
    [SerializeField] private bool _displayHour = true;
    [SerializeField] private string _currentHour;
    [SerializeField] private HourDisplay _hourDisplayPrefab;
    [Header("Dialogues")]
    [SerializeField] private DialogueDisplay _dialogueDisplayPrefab;

    private void Awake()
    {
        CreateInstance();
    }

    private void Start()
    {
        if (_displayHour) DisplayHour();
    }

    public void DisplayDialogueSequence(DialogueSequenceScriptable dialogueSequence)
    {
        DialogueDisplay dialogue = Instantiate(_dialogueDisplayPrefab);
        dialogue.DialogueSequence = dialogueSequence;
    }

    private void DisplayHour()
    {
        HourDisplay _hourDisplay = Instantiate(_hourDisplayPrefab);
        _hourDisplay.HourText = _currentHour;
        _hourDisplay.OnSelfDestroy += InvokeOnHourDisplayEnd;
    }

    public void DisplayNewQuest(QuestScriptable quest)
    {
        HUD.QuestDisplay.DisplayNewQuest(quest);
        quest.OnQuestEnd += RemoveQuest;
    }

    private void RemoveQuest(QuestScriptable quest)
    {
        CurrentQuests.Remove(quest);
    }

    private void InvokeOnHourDisplayEnd()
    {
        OnHourDisplayEnd?.Invoke();
    }

    private void CreateInstance()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one UIDisplayManager in the scene.");
        }
        Instance = this;
    }
}

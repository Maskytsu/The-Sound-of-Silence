using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [HideInInspector] public List<QuestScriptable> CurrentQuests = new List<QuestScriptable>();
    public event Action OnHourDisplayEnd;

    [SerializeField] private HourDisplay _hourDisplayPrefab;
    [SerializeField] private DialogueDisplay _dialogueDisplayPrefab;
    [SerializeField] private PauseMenu _pauseMenuPrefab;

    private GameManager GameManager => GameManager.Instance;

    private void Awake()
    {
        CreateInstance();
    }

    private void Start()
    {
        DisplayHour();
    }

    private void Update()
    {
        ManagePauseMenu();
    }

    public void DisplayDialogueSequence(DialogueSequenceScriptable dialogueSequence)
    {
        DialogueDisplay dialogue = Instantiate(_dialogueDisplayPrefab);
        dialogue.DialogueSequence = dialogueSequence;
    }

    public void DisplayNewQuest(QuestScriptable quest)
    {
        HUD.Instance.QuestDisplay.DisplayNewQuest(quest);
        quest.OnQuestEnd += RemoveQuest;
    }

    private void DisplayHour()
    {
        if (GameManager.IsGameplayScene)
        {
            HourDisplay _hourDisplay = Instantiate(_hourDisplayPrefab);
            _hourDisplay.HourText = GameManager.CurrentHour;
            _hourDisplay.OnSelfDestroy += () => OnHourDisplayEnd?.Invoke();
        }
    }

    private void RemoveQuest(QuestScriptable quest)
    {
        CurrentQuests.Remove(quest);
    }

    private void ManagePauseMenu()
    {
        if (InputProvider.Instance.PlayerMainMap.Pause.WasPerformedThisFrame())
        {
            PauseMenu pauseMenu = Instantiate(_pauseMenuPrefab);
        }
    }

    private void CreateInstance()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one UIManager in the scene.");
        }
        Instance = this;
    }
}

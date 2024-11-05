using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public event Action OnHourDisplayEnd;

    [SerializeField] private SceneSetup _sceneSetup;
    [Space]
    [SerializeField] private HourDisplay _hourDisplayPrefab;
    [SerializeField] private DialogueDisplay _dialogueDisplayPrefab;
    [SerializeField] private PauseMenu _pauseMenuPrefab;

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

    private void DisplayHour()
    {
        if (_sceneSetup.DisplayHour)
        {
            HourDisplay _hourDisplay = Instantiate(_hourDisplayPrefab);
            _hourDisplay.HourText = _sceneSetup.HourText;
            _hourDisplay.OnSelfDestroy += () => OnHourDisplayEnd?.Invoke();
        }
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

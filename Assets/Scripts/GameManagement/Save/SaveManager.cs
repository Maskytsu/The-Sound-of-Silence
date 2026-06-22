using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : SingletonMonobehaviour<SaveManager>
{
    [SerializeField] private SceneSetup _sceneSetup;
    [SerializeField] private GameState _gameState;
    [SerializeField] private Settings _settings;

    private List<BoolSaveData> _gameStateSaveData;
    private List<FloatSaveData> _settingsSaveData;
    private StringSaveData _currentSceneSaveData;

    //--------------------------------------
    private void InitializeSaveData()
    {
        _currentSceneSaveData = new("SavedScene", () => SceneManager.GetActiveScene().name, (string value) => SceneManager.LoadScene(PlayerPrefs.GetString(value)));

        _gameStateSaveData = new()
        {
            new ("MechanicChecked", () => _gameState.MechanicChecked, (bool value) => _gameState.MechanicChecked = value),
            new ("PoliceChecked", () => _gameState.PoliceChecked, (bool value) => _gameState.PoliceChecked = value),

            new ("MechanicMessaged", () => _gameState.MechanicMessaged, (bool value) => _gameState.MechanicMessaged = value),
            new ("ClaireMessaged", () => _gameState.ClaireMessaged, (bool value) => _gameState.ClaireMessaged = value),

            new ("ClaireCalled", () => _gameState.ClaireCalled, (bool value) => _gameState.ClaireCalled = value),
            new ("PoliceCalled", () => _gameState.PoliceCalled, (bool value) => _gameState.PoliceCalled = value),

            new ("TookKeys", () => _gameState.TookKeys, (bool value) => _gameState.TookKeys = value),
            new ("TookPills", () => _gameState.TookPills, (bool value) => _gameState.TookPills = value),

            new ("ReadConcertTicket", () => _gameState.ReadConcertTicket, (bool value) => _gameState.ReadConcertTicket = value),
            new ("ReadDivorcePapers", () => _gameState.ReadDivorcePapers, (bool value) => _gameState.ReadDivorcePapers = value),
            new ("ReadNewspaper", () => _gameState.ReadNewspaper, (bool value) => _gameState.ReadNewspaper = value)
        };

        _settingsSaveData = new()
        {
            new ("Volume", () => _settings.Volume, (float value) => _settings.Volume = value),
            new ("Brightness", () => _settings.Brightness, (float value) => _settings.Brightness = value)
        };
    }
    //--------------------------------------

    protected override void Awake()
    {
        InitializeSaveData();
        base.Awake();

        if (_sceneSetup.SaveSceneOnAwake)
        {
            SaveCurrentScene();
        }

        LoadGameState();
        LoadSettings();
    }

    public void ClearSave()
    {
        _currentSceneSaveData.ClearSavedValue();

        foreach (var saveDataElement in _gameStateSaveData)
        {
            saveDataElement.ClearSavedValue();
        }
    }

    //--------------------------------------
    public void SaveCurrentScene()
    {
        _currentSceneSaveData.SaveValue();
    }

    public void LoadSavedScene()
    {
        _currentSceneSaveData.LoadValue();
    }

    //--------------------------------------
    public void SaveGameState()
    {
        foreach (var saveDataElement in _gameStateSaveData)
        {
            saveDataElement.SaveValue();
        }
    }

    public void LoadGameState()
    {
        foreach (var saveDataElement in _gameStateSaveData)
        {
            saveDataElement.LoadValue();
        }
    }

    //--------------------------------------
    public void SaveSettings()
    {

        foreach (var saveDataElement in _settingsSaveData)
        {
            saveDataElement.SaveValue();
        }
    }

    public void LoadSettings()
    {
        foreach (var saveDataElement in _settingsSaveData)
        {
            saveDataElement.LoadValue();
        }
    }
}
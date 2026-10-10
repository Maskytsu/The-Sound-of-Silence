using System.Collections.Generic;
using Unity.Mathematics;
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
        _currentSceneSaveData = new("SavedScene", () => SceneManager.GetActiveScene().name, value => SceneManager.LoadScene(PlayerPrefs.GetString(value)));

        _gameStateSaveData = new()
        {
            new ("MechanicChecked", () => _gameState.MechanicChecked, value => _gameState.MechanicChecked = value),
            new ("PoliceChecked", () => _gameState.PoliceChecked, value => _gameState.PoliceChecked = value),

            new ("MechanicMessaged", () => _gameState.MechanicMessaged, value => _gameState.MechanicMessaged = value),
            new ("ClaireMessaged", () => _gameState.ClaireMessaged, value => _gameState.ClaireMessaged = value),

            new ("ClaireCalled", () => _gameState.ClaireCalled, value => _gameState.ClaireCalled = value),
            new ("PoliceCalled", () => _gameState.PoliceCalled, value => _gameState.PoliceCalled = value),

            new ("TookKeys", () => _gameState.TookKeys, value => _gameState.TookKeys = value),
            new ("TookPills", () => _gameState.TookPills, value => _gameState.TookPills = value),

            new ("ReadConcertTicket", () => _gameState.ReadConcertTicket, value => _gameState.ReadConcertTicket = value),
            new ("ReadDivorcePapers", () => _gameState.ReadDivorcePapers, value => _gameState.ReadDivorcePapers = value),
            new ("ReadNewspaper", () => _gameState.ReadNewspaper, value => _gameState.ReadNewspaper = value),

            new ("LeapUnlocked", () => _gameState.LeapUnlocked, value => _gameState.LeapUnlocked = value),
        };

        _settingsSaveData = new()
        {
            new ("GlobalVolume", () => _settings.GlobalVolume.Value, value => _settings.GlobalVolume.LoadValue(value), 0.75f),
            new ("SFXVolume", () => _settings.SFXVolume.Value, value => _settings.SFXVolume.LoadValue(value), 1.0f),
            new ("AmbientVolume", () => _settings.AmbientVolume.Value, value => _settings.AmbientVolume.LoadValue(value), 1.0f),
            new ("MusicVolume", () => _settings.MusicVolume.Value, value => _settings.MusicVolume.LoadValue(value), 1.0f),

            new ("Brightness", () => _settings.Brightness.Value, value => _settings.Brightness.LoadValue(value), 0.0f),
            new ("CameraSensitivity", () => _settings.CameraSensitivity.Value, value => _settings.CameraSensitivity.LoadValue(value), 0.5f),

            new ("VSyncOn", () => _settings.VSyncOn.Value ? 1 : 0, value => _settings.VSyncOn.LoadValue(Mathf.Approximately(value, 1)), 0),
            new ("BorderlessWindow", () => _settings.BorderlessWindow.Value ? 1 : 0, value => _settings.BorderlessWindow.LoadValue(Mathf.Approximately(value, 1)), 0),
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

        EndingsSaveManager.LoadEndings();
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
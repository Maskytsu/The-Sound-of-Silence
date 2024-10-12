using FMOD.Studio;
using FMODUnity;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [SerializeField] private GameManager _gameManager;
    [SerializeField] private GameState _gameState;
    [SerializeField] private Settings _settings;

    private void Awake()
    {
        CreateInstance();

        if (_gameManager.IsGameplayScene) SaveCurrentScene();
        LoadGameState();
        LoadSettings();
    }

    private void OnDestroy()
    {
        SaveGameState();
        //SaveSettings();
    }

    public void SaveCurrentScene()
    {
        PlayerPrefs.SetString("SavedScene", SceneManager.GetActiveScene().name);
    }

    public void SaveGameState()
    {
        PlayerPrefs.SetInt("CheckedMechanic", GameState.Instance.MechanicChecked ? 1 : 0);
        PlayerPrefs.SetInt("MessageSentToMechanic", GameState.Instance.MechanicMessaged ? 1 : 0);
        PlayerPrefs.SetInt("MessageSentToClaire", GameState.Instance.ClaireMessaged ? 1 : 0);
        PlayerPrefs.SetInt("CalledToClaire", GameState.Instance.ClaireCalled ? 1 : 0);
        PlayerPrefs.SetInt("CheckedPolice", GameState.Instance.PoliceChecked ? 1 : 0);
        PlayerPrefs.SetInt("CalledToPolice", GameState.Instance.PoliceCalled ? 1 : 0);
        PlayerPrefs.SetInt("TookPills", GameState.Instance.TookPills ? 1 : 0);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("Fullscreen", Settings.Instance.Fullscreen ? 1 : 0);
        PlayerPrefs.SetFloat("Brightness", Settings.Instance.Brightness);
        PlayerPrefs.SetFloat("Volume", Settings.Instance.Volume);
    }

    public void LoadSavedScene()
    {
        SceneManager.LoadScene(PlayerPrefs.GetString("SavedScene"));
    }

    public void LoadGameState()
    {
        _gameState.MechanicChecked = PlayerPrefs.GetInt("CheckedMechanic") == 1;
        _gameState.MechanicMessaged = PlayerPrefs.GetInt("MessageSentToMechanic") == 1;
        _gameState.ClaireMessaged = PlayerPrefs.GetInt("MessageSentToClaire") == 1;
        _gameState.ClaireCalled = PlayerPrefs.GetInt("CalledToClaire") == 1;
        _gameState.PoliceChecked = PlayerPrefs.GetInt("CheckedPolice") == 1;
        _gameState.PoliceCalled = PlayerPrefs.GetInt("CalledToPolice") == 1;
        _gameState.TookPills = PlayerPrefs.GetInt("TookPills") == 1;
    }

    public void LoadSettings()
    {
        _settings.Fullscreen = PlayerPrefs.GetInt("Fullscreen") == 1;
        _settings.Brightness = PlayerPrefs.GetFloat("Brightness");
        _settings.Volume = PlayerPrefs.GetFloat("Volume");
    }

    private void CreateInstance()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one SaveManager in the scene.");
        }
        Instance = this;
    }
}

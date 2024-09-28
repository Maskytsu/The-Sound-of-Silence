using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    [SerializeField] private bool _isGameplayScene = true;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one SaveManager in the scene.");
        }
        Instance = this;
    }

    private void Start()
    {
        if (_isGameplayScene) SaveGameState();
    }

    public void SaveGameState()
    {
        PlayerPrefs.SetString("SavedScene", SceneManager.GetActiveScene().name);

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

    public void LoadSceneFromSave()
    {
        SceneManager.LoadScene(PlayerPrefs.GetString("SavedScene"));
    }

    public void LoadGameState()
    {
        GameState.Instance.MechanicChecked = PlayerPrefs.GetInt("CheckedMechanic") == 1;
        GameState.Instance.MechanicMessaged = PlayerPrefs.GetInt("MessageSentToMechanic") == 1;
        GameState.Instance.ClaireMessaged = PlayerPrefs.GetInt("MessageSentToClaire") == 1;
        GameState.Instance.ClaireCalled = PlayerPrefs.GetInt("CalledToClaire") == 1;
        GameState.Instance.PoliceChecked = PlayerPrefs.GetInt("CheckedPolice") == 1;
        GameState.Instance.PoliceCalled = PlayerPrefs.GetInt("CalledToPolice") == 1;
        GameState.Instance.TookPills = PlayerPrefs.GetInt("TookPills") == 1;
    }

    public void LoadSettings()
    {
        Settings.Instance.Fullscreen = PlayerPrefs.GetInt("Fullscreen") == 1;
        Settings.Instance.Brightness = PlayerPrefs.GetFloat("Brightness");
        Settings.Instance.Volume = PlayerPrefs.GetFloat("Volume");
    }
}

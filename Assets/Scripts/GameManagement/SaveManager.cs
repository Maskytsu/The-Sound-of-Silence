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
        if (_isGameplayScene) SaveCurrentSceneName();
    }

    public void SaveCurrentSceneName()
    {
        PlayerPrefs.SetString("savedScene", SceneManager.GetActiveScene().name);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("fullscreen", Settings.Instance.Fullscreen ? 1 : 0);
        PlayerPrefs.SetFloat("brightness", Settings.Instance.Brightness);
        PlayerPrefs.SetFloat("volume", Settings.Instance.Volume);
    }

    public void LoadSceneFromSave()
    {
        SceneManager.LoadScene(PlayerPrefs.GetString("savedScene"));
    }

    public void LoadSettings()
    {
        Settings.Instance.Fullscreen = PlayerPrefs.GetInt("fullscreen") == 1;
        Settings.Instance.Brightness = PlayerPrefs.GetFloat("brightness");
        Settings.Instance.Volume = PlayerPrefs.GetFloat("volume");
    }
}

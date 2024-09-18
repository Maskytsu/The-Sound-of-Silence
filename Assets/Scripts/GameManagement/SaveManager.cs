using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one SaveManager in the scene.");
        }
        Instance = this;
    }

    public static void SaveGame()
    {
        //same as settings
    }

    public static void SaveSettings()
    {
        PlayerPrefs.SetInt("fullscreen", Settings.Instance.Fullscreen ? 1 : 0);
        PlayerPrefs.SetFloat("brightness", Settings.Instance.Brightness);
        PlayerPrefs.SetFloat("volume", Settings.Instance.Volume);
    }

    public static void LoadGame()
    {
        //same as settings
    }

    public static void LoadSettings()
    {
        Settings.Instance.Fullscreen = PlayerPrefs.GetInt("fullscreen") == 1;
        Settings.Instance.Brightness = PlayerPrefs.GetFloat("brightness");
        Settings.Instance.Volume = PlayerPrefs.GetFloat("volume");
    }
}

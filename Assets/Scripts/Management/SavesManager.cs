using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SavesManager : MonoBehaviour
{
    public static SavesManager instance { get; private set; }
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Saves Manager in the scene.");
        }
        instance = this;
    }

    public static void SaveGame()
    {
        //same as settings
    }

    public static void SaveSettings()
    {
        PlayerPrefs.SetInt("fullscreen", Settings.instance.fullscreen ? 1 : 0);
        PlayerPrefs.SetFloat("brightness", Settings.instance.brightness);
        PlayerPrefs.SetFloat("volume", Settings.instance.volume);
    }

    public static void LoadGame()
    {
        //same as settings
    }

    public static void LoadSettings()
    {
        Settings.instance.fullscreen = PlayerPrefs.GetInt("fullscreen") == 1;
        Settings.instance.brightness = PlayerPrefs.GetFloat("brightness");
        Settings.instance.volume = PlayerPrefs.GetFloat("volume");
    }
}

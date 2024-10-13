using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PhoneSetupScriptable CurrentPhoneSetup;

    public bool IsGameplayScene = true;

    public string CurrentHour;

    private void Awake()
    {
        CreateInstance();
    }

    private void CreateInstance()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one GameManager in the scene.");
        }
        Instance = this;
    }
}

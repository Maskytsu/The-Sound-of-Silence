using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [field: SerializeField] public PhoneSetupScriptable CurrentPhoneSetup { get; private set; }

    public bool IsGameplayScene = true;

    public string CurrentHour;

    private void Awake()
    {
        CreateInstance();
    }

    private void OnDestroy()
    {
        if (CurrentPhoneSetup != null) CurrentPhoneSetup.ClearSubscribersFromContacts();
    }

    public void ChangePhoneSetup(PhoneSetupScriptable phoneSetup)
    {
        CurrentPhoneSetup.ClearSubscribersFromContacts();
        CurrentPhoneSetup = phoneSetup;
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

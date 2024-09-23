using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public Transform UIParent;
    public PhoneSetupScriptable CurrentPhoneSetup;
    [HorizontalLine(color: EColor.Gray)]
    [SerializeField] private bool _displayHour = true;
    [SerializeField] private string _hour;
    [SerializeField] private HourDisplay _hourDisplayPrefab;

    private HourDisplay _hourDisplay;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one GameManager in the scene.");
        }
        Instance = this;

        if(_displayHour) DisplayHour();
    }

    private void DisplayHour()
    {
        _hourDisplay = Instantiate(_hourDisplayPrefab, UIParent);
        _hourDisplay.HourText = _hour;
    }
}

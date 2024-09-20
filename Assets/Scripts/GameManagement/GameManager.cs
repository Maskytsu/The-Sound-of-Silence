using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public HourDisplay HourDisplay;
    public Transform UIParent;
    [Space]
    [SerializeField] private bool _displayHour = true;
    [SerializeField] private string _hour;
    [SerializeField] private HourDisplay _hourDisplayPrefab;

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
        HourDisplay = Instantiate(_hourDisplayPrefab, UIParent);
        HourDisplay.HourText = _hour;
    }
}

using FMODUnity;
using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    public bool SaveSceneOnAwake;
    [Space]
    public bool IsAbleToHearOnAwake;
    [Space]
    public bool IsElectricityOnOnAwake;
    [Space]
    public bool LockCursor;
    [Space]
    public bool DisplayHour;
    [ShowIf(nameof(DisplayHour))] public string HourText;
    [Space]
    public bool HavePhone;
    public bool HaveFlashlight;
    public bool HaveKeys;
    public bool HaveGun;
    [Space]
    public PhoneSetupScriptable StartingPhoneSetup;
    [Space]
    public EventReference BackgroundMusic;
    public bool IsBackgroundMusic3D;
    public EventReference Ambient;
    public bool IsAmbient3D;
    [Space]
    public List<QuestScriptable> QuestSequence;
}

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
    public bool HaveShotgun;
    [Space]
    public PhoneSetupScriptable StartingPhoneSetup;
    [Space]
    public List<QuestScriptable> QuestSequence;
}

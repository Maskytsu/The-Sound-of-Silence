using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    [SerializeField] public bool SaveSceneOnAwake;
    [Space]
    [SerializeField] public bool IsAbleToHearOnAwake;
    [Space]
    [SerializeField] public bool DisplayHour;
    [ShowIf(nameof(DisplayHour)), SerializeField] public string HourText;
    [Space]
    [SerializeField] public bool ActivatePlayerMovementMap;
    [SerializeField] public bool ActivatePlayerMainMap;
    [SerializeField] public bool ActivateUIMap;
    [SerializeField] public bool LockCursor;
    [Space]
    [SerializeField] public PhoneSetupScriptable StartingPhoneSetup;
    [Space]
    [SerializeField] public List<QuestScriptable> QuestSequence;
}

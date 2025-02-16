using FMODUnity;
using NaughtyAttributes;
using UnityEngine;

public class FmodEvents : MonoBehaviour
{
    public static FmodEvents Instance { get; private set; }

    [field: Header("Hearing"), HorizontalLine(4)]
    [field: SerializeField] public EventReference H_OCC_MonsterAmbient { get; private set; }
    [field: SerializeField] public EventReference H_OCC_MonsterAngry { get; private set; }
    [field: SerializeField] public EventReference H_OCC_MonsterHit { get; private set; }
    [field: SerializeField] public EventReference H_OCC_MonsterPerish { get; private set; }
    [field: SerializeField] public EventReference H_OCC_MonsterTPCast { get; private set; }
    [field: SerializeField] public EventReference H_OCC_MonsterTPDone { get; private set; }
    [field: SerializeField] public EventReference H_OCC_OpeningWindow { get; private set; }
    [field: SerializeField] public EventReference H_OCC_Knocking { get; private set; }
    [field: SerializeField] public EventReference H_OCC_Creak { get; private set; }
    [field: Space(25)]
    [field: SerializeField] public EventReference H_SPT_MonsterMirrorSound { get; private set; }
    [field: SerializeField] public EventReference H_SPT_MonsterWhisper { get; private set; }
    [field: SerializeField] public EventReference H_SPT_ComingCar { get; private set; }
    [field: Space(25)]
    [field: SerializeField] public EventReference H_Calling { get; private set; }
    [field: SerializeField] public EventReference H_PlayerFootsteps { get; private set; }
    [field: SerializeField] public EventReference H_PlayerCruchesHit { get; private set; }
    [field: SerializeField] public EventReference H_GunShot { get; private set; }
    [field: SerializeField] public EventReference H_Thunder { get; private set; }
    [field: SerializeField] public EventReference H_MonsterTeaserSound { get; private set; }
    [field: Space(20)]
    [field: Header("Silence"), HorizontalLine(4)]
    [field: SerializeField] public EventReference S_SPT_MonsterMirrorSound { get; private set; }

    private void Awake()
    {
        CreateInstance();
    }

    private void CreateInstance()
    {
        if (Instance != null)
        {
            Debug.LogError("Found more than one FmodEvents in the scene.");
        }
        Instance = this;
    }
}
using FMODUnity;
using NaughtyAttributes;
using UnityEngine;

public class FmodEvents : MonoBehaviour
{
    public static FmodEvents Instance { get; private set; }
    //OCC - Occluded
    //SPT - Spatialized
    [field: SerializeField] public EventReference OCC_MonsterAmbient { get; private set; }
    [field: SerializeField] public EventReference OCC_MonsterAngry { get; private set; }
    [field: SerializeField] public EventReference OCC_MonsterHit { get; private set; }
    [field: SerializeField] public EventReference OCC_MonsterPerish { get; private set; }
    [field: SerializeField] public EventReference OCC_MonsterTPCast { get; private set; }
    [field: SerializeField] public EventReference OCC_MonsterTPDone { get; private set; }
    [field: SerializeField] public EventReference OCC_OpeningWindow { get; private set; }
    [field: SerializeField] public EventReference OCC_Knocking { get; private set; }
    [field: SerializeField] public EventReference OCC_Creak { get; private set; }
    [field: HorizontalLine(4f)]
    [field: SerializeField] public EventReference SPT_MonsterMirrorSound { get; private set; }
    [field: SerializeField] public EventReference SPT_MonsterWhisper { get; private set; }
    [field: SerializeField] public EventReference SPT_ComingCar { get; private set; }
    [field: SerializeField] public EventReference SPT_MovingDoor { get; private set; }
    [field: SerializeField] public EventReference SPT_MovingCabinetDoor { get; private set; }
    [field: SerializeField] public EventReference SPT_LightSwitchClick { get; private set; }
    [field: SerializeField] public EventReference SPT_ClosingWindow { get; private set; }
    [field: HorizontalLine(4f)]
    [field: SerializeField] public EventReference PlayerFootsteps { get; private set; }
    [field: SerializeField] public EventReference PlayerCruchesHit { get; private set; }
    [field: Space(10)]
    [field: SerializeField] public EventReference GunShot { get; private set; }
    [field: SerializeField] public EventReference KeysRumble { get; private set; }
    [field: SerializeField] public EventReference KeysUnlock { get; private set; }
    [field: SerializeField] public EventReference FlashlightClick { get; private set; }
    [field: SerializeField] public EventReference Calling { get; private set; }
    [field: Space(10)]
    [field: SerializeField] public EventReference Thunder { get; private set; }
    [field: SerializeField] public EventReference MonsterTeaserDreamSound { get; private set; }
    [field: Space(10)]
    [field: SerializeField] public EventReference PickingUpItem { get; private set; }
    [field: SerializeField] public EventReference SwallowingPills { get; private set; }
    [field: SerializeField] public EventReference DrinkingWater { get; private set; }
    [field: SerializeField] public EventReference GrabbingRope { get; private set; }
    [field: SerializeField] public EventReference ResetingBreakers { get; private set; }

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
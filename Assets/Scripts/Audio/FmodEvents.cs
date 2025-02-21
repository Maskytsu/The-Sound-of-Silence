using FMODUnity;
using NaughtyAttributes;
using UnityEngine;

public class FmodEvents : MonoBehaviour
{
    public static FmodEvents Instance { get; private set; }

    [field: Header("Occluded")]
    [field: SerializeField] public EventReference OCC_MonsterAmbient { get; private set; }
    [field: SerializeField] public EventReference OCC_MonsterAngry { get; private set; }
    [field: SerializeField] public EventReference OCC_MonsterHit { get; private set; } //always hearable
    [field: SerializeField] public EventReference OCC_MonsterPerish { get; private set; } //always hearable
    [field: SerializeField] public EventReference OCC_MonsterTPCast { get; private set; }
    [field: SerializeField] public EventReference OCC_MonsterTPDone { get; private set; }
    [field: SerializeField] public EventReference OCC_FridgeAmbient { get; private set; }
    [field: HorizontalLine(4f)]
    [field: Header("Spatialized")]
    [field: SerializeField] public EventReference SPT_MovingDoor { get; private set; }
    [field: SerializeField] public EventReference SPT_FenceGate { get; private set; }
    [field: SerializeField] public EventReference SPT_MovingCabinetDoor { get; private set; }
    [field: SerializeField] public EventReference SPT_LightSwitchClick { get; private set; }
    [field: SerializeField] public EventReference SPT_ClosingWindow { get; private set; }
    [field: SerializeField] public EventReference SPT_ComingCar { get; private set; }
    [field: SerializeField] public EventReference SPT_OpeningWindow { get; private set; }
    [field: SerializeField] public EventReference SPT_Knocking { get; private set; }
    [field: SerializeField] public EventReference SPT_MonsterWhisper { get; private set; }
    [field: SerializeField] public EventReference SPT_MonsterMirror { get; private set; } //always hearable
    [field: SerializeField] public EventReference SPT_Creak { get; private set; } //always hearable
    [field: HorizontalLine(4f)]
    [field: Header("2D")]
    //--------------------------------------using items
    [field: SerializeField] public EventReference GunShot { get; private set; }
    [field: SerializeField] public EventReference KeysRumble { get; private set; }
    [field: SerializeField] public EventReference KeysUnlock { get; private set; }
    [field: SerializeField] public EventReference FlashlightClick { get; private set; }
    [field: Space(15)]
    //--------------------------------------picking up / equipping items
    [field: SerializeField] public EventReference PickingUpFlashlight { get; private set; }
    [field: SerializeField] public EventReference PickingUpKeys { get; private set; }
    [field: SerializeField] public EventReference PickingUpGun { get; private set; }
    [field: SerializeField] public EventReference EquippingPhone { get; private set; }
    [field: SerializeField] public EventReference EquippingFlashlight { get; private set; }
    [field: SerializeField] public EventReference EquippingKeys { get; private set; }
    [field: SerializeField] public EventReference EquippingGun { get; private set; }
    [field: Space(15)]
    //--------------------------------------phone
    [field: SerializeField] public EventReference PhoneBackButton { get; private set; }
    [field: SerializeField] public EventReference PhoneSendButton { get; private set; }
    [field: SerializeField] public EventReference PhoneCallButton { get; private set; }
    [field: SerializeField] public EventReference PhoneContactButton { get; private set; }
    [field: SerializeField] public EventReference PhoneCalling { get; private set; }
    [field: SerializeField] public EventReference PhoneNewMessage { get; private set; }
    [field: Space(15)]
    //--------------------------------------interactions
    [field: SerializeField] public EventReference SwallowingPills { get; private set; }
    [field: SerializeField] public EventReference DrinkingWater { get; private set; }
    [field: SerializeField] public EventReference GrabbingRope { get; private set; }
    [field: SerializeField] public EventReference GrabbingCrutches { get; private set; }
    [field: SerializeField] public EventReference PuttingOnHearingAid { get; private set; } //always hearable
    [field: SerializeField] public EventReference ResetingBreakers { get; private set; }
    [field: SerializeField] public EventReference OpenToiletCover { get; private set; }
    [field: SerializeField] public EventReference CloseToiletCover { get; private set; }
    [field: SerializeField] public EventReference Flush { get; private set; }
    [field: Space(15)]
    //--------------------------------------animations etc.
    [field: SerializeField] public EventReference PlayerFootsteps { get; private set; }
    [field: SerializeField] public EventReference NewQuest { get; private set; }
    [field: SerializeField] public EventReference HiddenQuestAppeared { get; private set; }
    [field: SerializeField] public EventReference Thunder { get; private set; }
    [field: SerializeField] public EventReference TVShowMusic1 { get; private set; }
    [field: SerializeField] public EventReference TVShowMusic2 { get; private set; }
    [field: SerializeField] public EventReference TVPilotClick { get; private set; }
    [field: SerializeField] public EventReference CouchGettingUp { get; private set; }
    [field: SerializeField] public EventReference PuttingOffCrutches { get; private set; }
    [field: SerializeField] public EventReference TakingOffHearingAid { get; private set; } //always hearable
    [field: SerializeField] public EventReference BedGettingUp { get; private set; }
    [field: SerializeField] public EventReference BedFastGettingUp { get; private set; }
    [field: SerializeField] public EventReference StandingUp { get; private set; }
    [field: SerializeField] public EventReference MonsterTeaserDream { get; private set; }
    [field: SerializeField] public EventReference Peeing { get; private set; }
    [field: SerializeField] public EventReference CarCrash { get; private set; }
    [field: SerializeField] public EventReference LyingDownOnCouch { get; private set; }
    [field: SerializeField] public EventReference LyingDownOnBed { get; private set; }
    [field: HorizontalLine(4f)]
    [field: Header("2D, not pasuable, always hearable (mostly UI)")]
    [field: SerializeField] public EventReference OpenPaperSheet { get; private set; }
    [field: SerializeField] public EventReference ClosePaperSheet { get; private set; }

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
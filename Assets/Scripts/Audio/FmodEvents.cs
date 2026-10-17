using FMODUnity;
using NaughtyAttributes;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class FmodEvents : SingletonMonobehaviour<FmodEvents>
{
    const string headerOccluded = "Occluded";
    const string headerSpatialized = "Spatialized";
    const string header2dItems = "2D - items";
    const string header2dPhone = "2D - phone";
    const string header2dInteractions = "2D - interactions";
    const string header2dAnimationsAndOthers = "2D - animations and others";
    const string header2dUI = "2D - not pasuable, always hearable (mostly UI)";


    [TextArea, ReadOnly, SerializeField] string Info = "Green means that it must be always hearable in FMOD"; 

    [field: HorizontalLine(4f)]
    [field: SerializeField, Indent(2), Foldout(headerOccluded)] public EventReference OCC_MonsterAmbient { get; private set; }
    [field: SerializeField, Indent(2), Foldout(headerOccluded)] public EventReference OCC_MonsterAngry { get; private set; }
    [field: SerializeField, Indent(2), Foldout(headerOccluded), AlwaysHearableColor] public EventReference OCC_MonsterHit { get; private set; }
    [field: SerializeField, Indent(2), Foldout(headerOccluded), AlwaysHearableColor] public EventReference OCC_MonsterPerish { get; private set; }
    [field: SerializeField, Indent(2), Foldout(headerOccluded)] public EventReference OCC_MonsterTPCast { get; private set; }
    [field: SerializeField, Indent(2), Foldout(headerOccluded)] public EventReference OCC_MonsterTPDone { get; private set; }
    [field: SerializeField, Indent(2), Foldout(headerOccluded)] public EventReference OCC_FridgeAmbient { get; private set; }

    [field: HorizontalLine(4f)]
    [field: SerializeField, Indent(2), Foldout(headerSpatialized)] public EventReference SPT_MovingDoor { get; private set; }
    [field: SerializeField, Indent(2), Foldout(headerSpatialized)] public EventReference SPT_FenceGate { get; private set; }
    [field: SerializeField, Indent(2), Foldout(headerSpatialized)] public EventReference SPT_MovingCabinetDoor { get; private set; }
    [field: SerializeField, Indent(2), Foldout(headerSpatialized)] public EventReference SPT_LightSwitchClick { get; private set; }
    [field: SerializeField, Indent(2), Foldout(headerSpatialized)] public EventReference SPT_ClosingWindow { get; private set; }
    [field: SerializeField, Indent(2), Foldout(headerSpatialized)] public EventReference SPT_ComingCar { get; private set; }
    [field: SerializeField, Indent(2), Foldout(headerSpatialized)] public EventReference SPT_OpeningWindow { get; private set; }
    [field: SerializeField, Indent(2), Foldout(headerSpatialized)] public EventReference SPT_Knocking { get; private set; }
    [field: SerializeField, Indent(2), Foldout(headerSpatialized)] public EventReference SPT_MonsterWhisper { get; private set; }
    [field: SerializeField, Indent(2), Foldout(headerSpatialized), AlwaysHearableColor] public EventReference SPT_MonsterMirror { get; private set; }
    [field: SerializeField, Indent(2), Foldout(headerSpatialized), AlwaysHearableColor] public EventReference SPT_Creak { get; private set; }

    [field: HorizontalLine(4f)]
    [field: SerializeField, Indent(2), Foldout(header2dItems)] public EventReference GunShot { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dItems)] public EventReference KeysRumble { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dItems)] public EventReference KeysUnlock { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dItems)] public EventReference FlashlightClickOff { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dItems)] public EventReference FlashlightClickOn { get; private set; }

    [field: Space]
    [field: SerializeField, Indent(2), Foldout(header2dItems)] public EventReference PickingUpKeys { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dItems)] public EventReference EquippingPhone { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dItems)] public EventReference EquippingFlashlight { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dItems)] public EventReference EquippingKeys { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dItems)] public EventReference EquippingGun { get; private set; }

    [field: HorizontalLine(4f)]
    [field: SerializeField, Indent(2), Foldout(header2dPhone)] public EventReference PhoneBackButton { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dPhone)] public EventReference PhoneSendButton { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dPhone)] public EventReference PhoneCallButton { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dPhone)] public EventReference PhoneContactButton { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dPhone)] public EventReference PhoneCalling { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dPhone)] public EventReference PhoneNewMessage { get; private set; }

    [field: HorizontalLine(4f)]
    [field: SerializeField, Indent(2), Foldout(header2dInteractions)] public EventReference SwallowingPills { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dInteractions)] public EventReference DrinkingWater { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dInteractions)] public EventReference GrabbingRope { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dInteractions)] public EventReference GrabbingCrutches { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dInteractions), AlwaysHearableColor] public EventReference PuttingOnHearingAid { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dInteractions)] public EventReference ResetingBreakers { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dInteractions)] public EventReference OpenToiletCover { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dInteractions)] public EventReference CloseToiletCover { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dInteractions)] public EventReference Flush { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dInteractions)] public EventReference Piano { get; private set; }

    [field: HorizontalLine(4f)]
    [field: SerializeField, Indent(2), Foldout(header2dAnimationsAndOthers)] public EventReference PlayerFootsteps { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dAnimationsAndOthers)] public EventReference NewQuest { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dAnimationsAndOthers)] public EventReference Thunder { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dAnimationsAndOthers)] public EventReference TVShowMusic1 { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dAnimationsAndOthers)] public EventReference TVShowMusic2 { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dAnimationsAndOthers)] public EventReference TVPilotClick { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dAnimationsAndOthers)] public EventReference CouchGettingUp { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dAnimationsAndOthers)] public EventReference PuttingOffCrutches { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dAnimationsAndOthers), AlwaysHearableColor] public EventReference TakingOffHearingAid { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dAnimationsAndOthers)] public EventReference BedGettingUp { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dAnimationsAndOthers)] public EventReference BedFastGettingUp { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dAnimationsAndOthers)] public EventReference StandingUp { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dAnimationsAndOthers)] public EventReference MonsterTeaserDream { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dAnimationsAndOthers)] public EventReference Peeing { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dAnimationsAndOthers)] public EventReference CarBraking { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dAnimationsAndOthers)] public EventReference CarCrash { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dAnimationsAndOthers)] public EventReference LyingDownOnCouch { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dAnimationsAndOthers)] public EventReference LyingDownOnBed { get; private set; }

    [field: HorizontalLine(4f)]
    [field: SerializeField, Indent(2), Foldout(header2dUI)] public EventReference OpenPaperSheet { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dUI)] public EventReference ClosePaperSheet { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dUI)] public EventReference UIQuitGame { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dUI)] public EventReference UIHover { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dUI)] public EventReference UIAccept { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dUI)] public EventReference UIDecline { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dUI)] public EventReference UIOpenPauseMenu { get; private set; }
    [field: SerializeField, Indent(2), Foldout(header2dUI)] public EventReference UIClosePauseMenu { get; private set; }
}

//------------------------------------------------------------------------------
#if UNITY_EDITOR
public class AlwaysHearableColorAttribute : PropertyAttribute
{
    public Color Color { get; } = Color.green;
}

[CustomPropertyDrawer(typeof(AlwaysHearableColorAttribute))]
public class ColoredFieldDrawer : PropertyDrawer
{
    public override void OnGUI(
        Rect position,
        SerializedProperty property,
        GUIContent label)
    {
        var attribute = (AlwaysHearableColorAttribute)this.attribute;

        var oldColor = GUI.contentColor;
        GUI.contentColor = attribute.Color;

        EditorGUI.PropertyField(position, property, label, true);

        GUI.contentColor = oldColor;
    }

    public override float GetPropertyHeight(
        SerializedProperty property,
        GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}

//------------------------------------------------------------------------------
[CustomPropertyDrawer(typeof(IndentAttribute))]
public class IndentDrawer : PropertyDrawer
{
    public override void OnGUI(
        Rect position,
        SerializedProperty property,
        GUIContent label)
    {
        var indent = (IndentAttribute)attribute;

        int oldIndent = EditorGUI.indentLevel;
        EditorGUI.indentLevel += indent.IndentLevel;

        EditorGUI.PropertyField(position, property, label, true);

        EditorGUI.indentLevel = oldIndent;
    }

    public override float GetPropertyHeight(
        SerializedProperty property,
        GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
}
public class IndentAttribute : PropertyAttribute
{
    public int IndentLevel { get; }

    public IndentAttribute(int indentLevel = 1)
    {
        IndentLevel = indentLevel;
    }
}

#endif
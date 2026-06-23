using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine.UIElements;

[Overlay(typeof(SceneView), "TSoS")]
public class SceneViewGizmoSettings : Overlay
{
    public static bool DrawTriggers { get => EditorPrefs.GetBool("DrawTriggers", true); set => EditorPrefs.SetBool("DrawTriggers", value); }
    public static bool DrawFullTriggers { get => EditorPrefs.GetBool("DrawFullTriggers", false); set => EditorPrefs.SetBool("DrawFullTriggers", value); }
    public static bool DrawPlayer { get => EditorPrefs.GetBool("DrawPlayer", true); set => EditorPrefs.SetBool("DrawPlayer", value); }
    public static bool DrawPlayerTargetPos { get => EditorPrefs.GetBool("DrawPlayerTargetPos", true); set => EditorPrefs.SetBool("DrawPlayerTargetPos", value); }
    public static bool DrawInteractableGizmo { get => EditorPrefs.GetBool("DrawInteractableGizmo", true); set => EditorPrefs.SetBool("DrawInteractableGizmo", value); }
    public static bool DivideInteractableGizmo { get => EditorPrefs.GetBool("DivideInteractableGizmo", true); set => EditorPrefs.SetBool("DivideInteractableGizmo", value); }
    public static bool DrawInteractionHitbox { get => EditorPrefs.GetBool("DrawInteractionHitbox", false); set => EditorPrefs.SetBool("DrawInteractionHitbox", value); }

    public override VisualElement CreatePanelContent()
    {
        var toggleDrawTriggers = new Toggle("Draw Triggers");
        var toggleDrawFullTriggers = new Toggle("--Draw Full Triggers");
        var toggleDrawPlayer = new Toggle("Draw Player");
        var toggleDrawPlayerTargetPos = new Toggle("Draw Player Target Pos");
        var toggleDrawInteractableGizmo = new Toggle("Draw Interactable Gizmo");
        var toggleDivideInteractableGizmo = new Toggle("--Divide Interactable Gizmo");
        var toggleDrawInteractionHitbox = new Toggle("Draw Interaction Hitbox");

        toggleDrawTriggers.value = DrawTriggers;
        toggleDrawFullTriggers.value = DrawFullTriggers;
        toggleDrawPlayer.value = DrawPlayer;
        toggleDrawPlayerTargetPos.value = DrawPlayerTargetPos;
        toggleDrawInteractableGizmo.value = DrawInteractableGizmo;
        toggleDivideInteractableGizmo.value = DivideInteractableGizmo;
        toggleDrawInteractionHitbox.value = DrawInteractionHitbox;

        toggleDrawTriggers.RegisterValueChangedCallback(evt => DrawTriggers = evt.newValue);
        toggleDrawFullTriggers.RegisterValueChangedCallback(evt => DrawFullTriggers = evt.newValue);
        toggleDrawPlayer.RegisterValueChangedCallback(evt => DrawPlayer = evt.newValue);
        toggleDrawPlayerTargetPos.RegisterValueChangedCallback(evt => DrawPlayerTargetPos = evt.newValue);
        toggleDrawInteractableGizmo.RegisterValueChangedCallback(evt => DrawInteractableGizmo = evt.newValue);
        toggleDivideInteractableGizmo.RegisterValueChangedCallback(evt => DivideInteractableGizmo = evt.newValue);
        toggleDrawInteractionHitbox.RegisterValueChangedCallback(evt => DrawInteractionHitbox = evt.newValue);

        var root = new VisualElement();

        root.Add(toggleDrawTriggers);
        if (DrawTriggers) root.Add(toggleDrawFullTriggers);
        root.Add(toggleDrawPlayer);
        root.Add(toggleDrawPlayerTargetPos);
        root.Add(toggleDrawInteractableGizmo);
        if (DrawInteractableGizmo) root.Add(toggleDivideInteractableGizmo);
        root.Add(toggleDrawInteractionHitbox);

        return root;
    }
}
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
    public static bool DrawInteractionHitbox { get => EditorPrefs.GetBool("DrawInteractionHitbox", false); set => EditorPrefs.SetBool("DrawInteractionHitbox", value); }
    public static bool DrawInteractableGizmo { get => EditorPrefs.GetBool("DrawInteractableGizmo", true); set => EditorPrefs.SetBool("DrawInteractableGizmo", value); }
    public static bool DivideInteractableGizmo { get => EditorPrefs.GetBool("DivideInteractableGizmo", true); set => EditorPrefs.SetBool("DivideInteractableGizmo", value); }

    public override VisualElement CreatePanelContent()
    {
        var toggleDrawTriggers = new Toggle("Draw Triggers");
        var toggleDrawFullTriggers = new Toggle("--Draw Full Triggers");
        var toggleDrawPlayer = new Toggle("Draw Player");
        var toggleDrawPlayerTargetPos = new Toggle("Draw Player Target Pos");
        var toggleDrawInteractionHitbox = new Toggle("Draw Interaction Hitbox");
        var toggleDrawInteractableGizmo = new Toggle("Draw Interactable Gizmo");
        var toggleDivideInteractableGizmo = new Toggle("--Divide Interactable Gizmo");

        toggleDrawTriggers.value = DrawTriggers;
        toggleDrawFullTriggers.value = DrawFullTriggers;
        toggleDrawPlayer.value = DrawPlayer;
        toggleDrawPlayerTargetPos.value = DrawPlayerTargetPos;
        toggleDrawInteractionHitbox.value = DrawInteractionHitbox;
        toggleDrawInteractableGizmo.value = DrawInteractableGizmo;
        toggleDivideInteractableGizmo.value = DivideInteractableGizmo;

        toggleDrawTriggers.RegisterValueChangedCallback(evt => DrawTriggers = evt.newValue);
        toggleDrawFullTriggers.RegisterValueChangedCallback(evt => DrawFullTriggers = evt.newValue);
        toggleDrawPlayer.RegisterValueChangedCallback(evt => DrawPlayer = evt.newValue);
        toggleDrawPlayerTargetPos.RegisterValueChangedCallback(evt => DrawPlayerTargetPos = evt.newValue);
        toggleDrawInteractionHitbox.RegisterValueChangedCallback(evt => DrawInteractionHitbox = evt.newValue);
        toggleDrawInteractableGizmo.RegisterValueChangedCallback(evt => DrawInteractableGizmo = evt.newValue);
        toggleDivideInteractableGizmo.RegisterValueChangedCallback(evt => DivideInteractableGizmo = evt.newValue);

        toggleDrawFullTriggers.style.marginBottom = 15;
        toggleDrawPlayerTargetPos.style.marginBottom = 15;

        var root = new VisualElement();

        root.Add(toggleDrawTriggers);
        root.Add(toggleDrawFullTriggers);
        root.Add(toggleDrawPlayer);
        root.Add(toggleDrawPlayerTargetPos);
        root.Add(toggleDrawInteractionHitbox);
        root.Add(toggleDrawInteractableGizmo);
        root.Add(toggleDivideInteractableGizmo);

        return root;
    }
}
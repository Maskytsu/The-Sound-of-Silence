using UnityEditor;
using UnityEngine;

public class DebugInfoDescriptionWindow : EditorWindow
{
    private string displayText =
        "Bindingi:\n" +
        "M - toggle monster interactions\n" +
        "Shift - toggle sprint\n" +
        "U - toggle time scale na 50\n" +
        "N - toggle no clip\n\n" +
        "Ending setup - pod GameManagerem mamy obiekt GameplayManager i na nim komponent GameState, a tam są guziki\n" +
        "Ogólnie można to przejrzeć, bo tam są różne guziki na obiektach pod GameManagerem ;)\n\n" +
        "Na scriptable'ach questów można je zaczynać i kończyć guzikiem\n\n" +
        "Gizmosy:\n" +
        "- trigger: player (pink), monster (yellow)\n" +
        "- interactable: glitch (pink), pickable (yellow), simple dialogue (green), lock (red), disabled (transparent)";

    [MenuItem("TSoS/Debug Info")]
    public static void ShowWindow()
    {
        GetWindow<DebugInfoDescriptionWindow>("Debug Info");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField(displayText, EditorStyles.wordWrappedLabel);
    }
}
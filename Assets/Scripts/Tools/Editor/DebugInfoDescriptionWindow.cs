using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DebugInfoDescriptionWindow : EditorWindow
{
    [MenuItem("TSoS/Debug Info")]
    public static void ShowWindow()
    {
        GetWindow<DebugInfoDescriptionWindow>("Debug Info");
    }

    private void OnGUI()
    {
        var setup = Resources.Load("DebugInfoSetup") as DebugInfoDescriptionSetup;
        string text = "";
        foreach (var line in setup.DebugInfo)
        {
            text += line.Replace("\\n", "\n") + "\n\n";
        }
        EditorGUILayout.LabelField(text, EditorStyles.wordWrappedLabel);
    }
}
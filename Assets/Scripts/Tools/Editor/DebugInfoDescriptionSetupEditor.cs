using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

//this is some AI slop
[CustomEditor(typeof(DebugInfoDescriptionSetup))]
public class DebugInfoDescriptionSetupEditor : Editor
{
    private ReorderableList list;
    private SerializedProperty values;

    private void OnEnable()
    {
        values = serializedObject.FindProperty(nameof(DebugInfoDescriptionSetup.DebugInfo));

        list = new ReorderableList(serializedObject, values, true, true, true, true);

        list.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(rect, "Debug Info");
        };

        list.elementHeight = 105;

        list.drawElementCallback = (rect, index, active, focused) =>
        {
            SerializedProperty element = values.GetArrayElementAtIndex(index);

            rect.y += 2;
            rect.height = 100;

            element.stringValue = EditorGUI.TextArea(rect, element.stringValue);
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
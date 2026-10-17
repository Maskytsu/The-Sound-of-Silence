using NaughtyAttributes.Editor;
using UnityEditor;
using UnityEngine;
using Line = DialogueSequenceScriptable.DialogueLine;

[CustomEditor(typeof(DialogueSequenceScriptable))]
public class DialogueSequenceScriptableEditor : NaughtyInspector
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        EditorGUI.BeginChangeCheck();
        DialogueSequenceScriptable.EditingEvents = EditorGUILayout.Toggle("Editing Events", DialogueSequenceScriptable.EditingEvents);
        if (EditorGUI.EndChangeCheck()) Repaint();
    }
}

[CustomPropertyDrawer(typeof(Line))]
public class DialogueLineDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label, true);
        rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            ForEachVisibleChild(property, child =>
            {
                float h = EditorGUI.GetPropertyHeight(child, true);
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, h), child, true);
                rect.y += h + EditorGUIUtility.standardVerticalSpacing;
            });
            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        if (property.isExpanded)
        {
            ForEachVisibleChild(property, child =>
                height += EditorGUI.GetPropertyHeight(child, true) + EditorGUIUtility.standardVerticalSpacing);
        }
        return height;
    }

    static void ForEachVisibleChild(SerializedProperty property, System.Action<SerializedProperty> action)
    {
        bool isEvent = property.FindPropertyRelative(nameof(Line.IsEvent)).boolValue;

        var it = property.Copy();
        var end = property.GetEndProperty();
        bool enterChildren = true;

        while (it.NextVisible(enterChildren) && !SerializedProperty.EqualContents(it, end))
        {
            enterChildren = false;

            if (it.name == nameof(Line.IsEvent) && !DialogueSequenceScriptable.EditingEvents)
                continue;

            if (it.name == nameof(Line.Text) && isEvent)
                continue;

            if (it.name == nameof(Line.ColorType) && isEvent)
                continue;

            if (it.name == nameof(Line.Event) && !isEvent)
                continue;

            action(it);
        }
    }
}
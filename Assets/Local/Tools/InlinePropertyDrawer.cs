#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Draws an object inspector GUI nested within the parent inspector.
/// </summary>
[CustomPropertyDrawer(typeof(InlineAttribute))]
public class InlinePropertyDrawer : PropertyDrawer {
    private Editor _editor;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);
        
        bool hasValue = property.objectReferenceValue != null;
        float badgeSize = hasValue ? 46f : 0f;
        Rect fieldRect = new Rect(position.x, position.y, position.width - badgeSize, EditorGUIUtility.singleLineHeight);
        
        // Foldout arrow
        if (property.objectReferenceValue != null) {
            property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, 12, EditorGUIUtility.singleLineHeight), property.isExpanded, GUIContent.none, true);
        }
        
        // Draw the reference field
        EditorGUI.PropertyField(fieldRect, property, label);

        if (hasValue) {
            GUIStyle inlinedStyle = new GUIStyle(EditorStyles.miniLabel) {
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = new Color(0.2f, 0.8f, 0.2f, 0.8f) },
                fontStyle = FontStyle.Bold
            };
            
            Rect badgeRect = new Rect(position.xMax - badgeSize + 8, position.y, badgeSize, EditorGUIUtility.singleLineHeight);
            GUI.Label(badgeRect, "inlined", inlinedStyle);
        }

        if (property.objectReferenceValue != null && property.isExpanded) {
            // Create or update the cached editor
            Editor.CreateCachedEditor(property.objectReferenceValue, null, ref _editor);

            if (_editor != null) {
                float headerHeight = EditorGUIUtility.singleLineHeight + 4;
                Rect boxRect = new Rect(position.x, position.y + headerHeight, position.width, position.height - headerHeight);
                
                // Draw a nice box around the nested content
                GUI.Box(boxRect, GUIContent.none, EditorStyles.helpBox);

                EditorGUI.indentLevel++;
                SerializedObject so = _editor.serializedObject;
                so.Update();

                SerializedProperty p = so.GetIterator();
                p.NextVisible(true); // Skip 'm_Script' field

                float currentY = position.y + headerHeight + 5;
                while (p.NextVisible(false)) {
                    float propHeight = EditorGUI.GetPropertyHeight(p, true);
                    // Indent content manually to fit inside the box
                    Rect propRect = new Rect(position.x + 5, currentY, position.width - 10, propHeight);
                    EditorGUI.PropertyField(propRect, p, true);
                    currentY += propHeight + EditorGUIUtility.standardVerticalSpacing;
                }

                so.ApplyModifiedProperties();
                EditorGUI.indentLevel--;
            }
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        float height = EditorGUIUtility.singleLineHeight;
        
        if (property.objectReferenceValue != null && property.isExpanded) {
            // We need to calculate the height of all visible properties
            SerializedObject so = new SerializedObject(property.objectReferenceValue);
            SerializedProperty p = so.GetIterator();
            p.NextVisible(true);
            
            height += 10; // Top/bottom padding
            while (p.NextVisible(false)) {
                height += EditorGUI.GetPropertyHeight(p, true) + EditorGUIUtility.standardVerticalSpacing;
            }
        }
        
        return height;
    }
}
#endif

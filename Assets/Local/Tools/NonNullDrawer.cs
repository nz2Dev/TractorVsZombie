using UnityEditor;

using UnityEngine;

[CustomPropertyDrawer(typeof(NonNullAttribute))]
public class NotNullDrawer : PropertyDrawer {
    private const float Spacing = 2f;
    private const float HelpBoxHeight = 38f;

    public override float GetPropertyHeight(
        SerializedProperty property,
        GUIContent label) {
        float propertyHeight = EditorGUI.GetPropertyHeight(property, label, true);

        if (property.objectReferenceValue != null)
            return propertyHeight;

        return propertyHeight + Spacing + HelpBoxHeight;
    }

    public override void OnGUI(
        Rect position,
        SerializedProperty property,
        GUIContent label) {
        float propertyHeight = EditorGUI.GetPropertyHeight(property, label, true);

        Rect propertyRect = new Rect(
            position.x,
            position.y,
            position.width,
            propertyHeight
        );

        EditorGUI.PropertyField(
            propertyRect,
            property,
            label,
            true
        );

        if (property.objectReferenceValue == null) {
            Rect helpBoxRect = new Rect(
                position.x,
                position.y + propertyHeight + Spacing,
                position.width,
                HelpBoxHeight
            );

            EditorGUI.HelpBox(
                helpBoxRect,
                $"{label.text} must not be null.",
                MessageType.Error
            );
        }
    }
}
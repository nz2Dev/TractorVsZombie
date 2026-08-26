using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(LocalAttribute))]
public class LocalPropertyDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);

        bool isLocal = CheckIsLocal(property);
        Object previousValue = property.objectReferenceValue;
        Rect fieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        
        EditorGUI.BeginChangeCheck();

        if (!isLocal) {
            Color originalColor = GUI.color;
            GUI.color = new Color(1f, 0.4f, 0.4f); // Noticeable red
            EditorGUI.PropertyField(fieldRect, property, label);
            GUI.color = originalColor;

            Rect helpBoxRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, EditorGUIUtility.singleLineHeight * 1.5f);
            EditorGUI.HelpBox(helpBoxRect, "Object must be the same GameObject or a child.", MessageType.Error);
        } else {
            if (property.objectReferenceValue != null) {
                float tagWidth = 45f;
                Rect propertyRect = new Rect(position.x, position.y, position.width - tagWidth, EditorGUIUtility.singleLineHeight);
                Rect tagRect = new Rect(position.x + position.width - tagWidth, position.y, tagWidth, EditorGUIUtility.singleLineHeight);

                EditorGUI.PropertyField(propertyRect, property, label);
                
                GUIStyle tagStyle = new GUIStyle(EditorStyles.miniLabel);
                tagStyle.normal.textColor = new Color(0.3f, 0.8f, 0.3f);
                tagStyle.alignment = TextAnchor.MiddleRight;
                EditorGUI.LabelField(tagRect, "is local", tagStyle);
            } else {
                EditorGUI.PropertyField(fieldRect, property, label);
            }
        }

        if (EditorGUI.EndChangeCheck()) {
            HandleAutoInstantiation(property, previousValue);
        }

        EditorGUI.EndProperty();
    }

    private void HandleAutoInstantiation(SerializedProperty property, Object previousValue) {
        Object val = property.objectReferenceValue;
        if (val == null) return;

        // Check if it's a prefab asset (not a scene instance)
        if (PrefabUtility.IsPartOfPrefabAsset(val) && EditorUtility.IsPersistent(val)) {
            GameObject prefabGo = val as GameObject;
            if (prefabGo == null && val is Component comp) prefabGo = comp.gameObject;

            if (prefabGo != null) {
                Component owner = property.serializedObject.targetObject as Component;
                if (owner == null) return;

                // Identify the previous object to see if it should be destroyed
                GameObject oldGo = null;
                if (previousValue != null && !EditorUtility.IsPersistent(previousValue)) {
                    oldGo = previousValue as GameObject;
                    if (oldGo == null && previousValue is Component comp1) oldGo = comp1.gameObject;
                }

                // Instantiate as child
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefabGo);
                Undo.RegisterCreatedObjectUndo(instance, "Auto Instantiate Local Prefab");
                
                instance.transform.SetParent(owner.transform, false);
                instance.transform.localPosition = Vector3.zero;
                instance.transform.localRotation = Quaternion.identity;
                instance.name = prefabGo.name;

                // Destroy the old object if it was a scene instance and not the owner itself
                if (oldGo != null && oldGo != owner.gameObject) {
                    Undo.DestroyObjectImmediate(oldGo);
                }

                // Set focus to the new instance so it's obvious what happened
                EditorGUIUtility.PingObject(instance);

                // Update property to reference the instance instead of the prefab asset
                if (val is GameObject) {
                    property.objectReferenceValue = instance;
                } else {
                    property.objectReferenceValue = instance.GetComponent(val.GetType());
                }
                
                property.serializedObject.ApplyModifiedProperties();
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        bool isLocal = CheckIsLocal(property);
        if (!isLocal) {
            return EditorGUIUtility.singleLineHeight * 2.5f + 4;
        }
        return EditorGUIUtility.singleLineHeight;
    }

    private bool CheckIsLocal(SerializedProperty property) {
        Object objRef = property.objectReferenceValue;
        if (objRef == null) return true;

        Component owner = property.serializedObject.targetObject as Component;
        if (owner == null) return true;

        GameObject referencedGo = objRef as GameObject;
        if (referencedGo == null && objRef is Component comp) {
            referencedGo = comp.gameObject;
        }

        if (referencedGo == null) return true;

        // Check if it's the same object or a child
        return referencedGo.transform.IsChildOf(owner.transform);
    }
}

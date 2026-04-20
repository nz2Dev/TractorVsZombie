using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InfantryAuthoring))]
public class InfantryAuthoringEditor : Editor {
    InfantryAuthoring provider;

    private Editor scriptableEditor;

    void OnEnable() {
        provider = target as InfantryAuthoring;
    }

    private void OnDisable() {
        if (scriptableEditor != null) {
            DestroyImmediate(scriptableEditor);
        }
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        if (provider == null)
            return;

        EditorGUILayout.Space();

        if (provider.infantryConfig != null) {
            // Create cached editor for ScriptableObject
            if (scriptableEditor == null) {
                scriptableEditor = CreateEditor(provider.infantryConfig);
            }

            EditorGUILayout.LabelField("Scriptable Object", EditorStyles.boldLabel);

            // Draw ScriptableObject inspector inside MonoBehaviour inspector
            scriptableEditor.OnInspectorGUI();
        }

        GUILayout.Space(10);

        using (new EditorGUILayout.VerticalScope("box")) {
            GUILayout.Label("Prefab Authoring", EditorStyles.boldLabel);

            var prefabs = provider.ListEditablePrefabs();
            if (prefabs == null)
                return;

            int index = 0;
            foreach (var prefab in prefabs) {
                if (prefab == null)
                    continue;
                DrawPrefabRow(prefab, index++);
            }
        }
    }

    void DrawPrefabRow(GameObject prefab, int index) {
        var instance = FindInstanceForPrefab(prefab);

        using (new EditorGUILayout.HorizontalScope()) {
            using (new EditorGUI.DisabledScope(true)) {
                EditorGUILayout.ObjectField(prefab, typeof(GameObject), false);
            }

            if (instance == null) {
                if (GUILayout.Button("Spawn", GUILayout.Width(80))) {
                    Spawn(prefab);
                }
            } else {
                if (GUILayout.Button("Despawn", GUILayout.Width(80))) {
                    Despawn(instance);
                }
            }
        }
    }

    GameObject FindInstanceForPrefab(GameObject prefab) {
        if (provider == null || prefab == null)
            return null;

        for (int i = 0; i < provider.transform.childCount; i++) {
            var child = provider.transform.GetChild(i).gameObject;
            var source = PrefabUtility.GetCorrespondingObjectFromSource(child);
            if (source == prefab) {
                return child;
            }
        }
        return null;
    }

    void Spawn(GameObject prefab) {
        var instance = (GameObject) PrefabUtility.InstantiatePrefab(prefab, provider.transform);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localRotation = Quaternion.identity;

        Undo.RegisterCreatedObjectUndo(instance, "Spawn Editable Prefab");

        if (provider.spawnedInstances == null)
            provider.spawnedInstances = new System.Collections.Generic.List<GameObject>();

        provider.spawnedInstances.Add(instance);
        EditorUtility.SetDirty(provider);

        Selection.activeGameObject = instance;
    }

    void Despawn(GameObject instance) {
        if (instance == null)
            return;

        if (provider.spawnedInstances != null && provider.spawnedInstances.Contains(instance)) {
            provider.spawnedInstances.Remove(instance);
            EditorUtility.SetDirty(provider);
        }

        Undo.DestroyObjectImmediate(instance);
    }
}
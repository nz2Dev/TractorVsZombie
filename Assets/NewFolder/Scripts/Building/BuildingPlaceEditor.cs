using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BuildingPlace))]
public class BuildingPlaceEditor : Editor {

    BuildingPlace provider;
    SerializedProperty spawnedInstanceProp;

    void OnEnable() {
        provider = target as BuildingPlace;

        // Optional: track spawned instance if field exists
        spawnedInstanceProp = serializedObject.FindProperty("spawnedInstance");
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        if (provider == null)
            return;

        GUILayout.Space(10);

        using (new EditorGUILayout.VerticalScope("box")) {
            GUILayout.Label("Prefab Authoring", EditorStyles.boldLabel);

            var prefab = provider.GetBuildingTypeConfigVisualsPrefab();

            if (prefab == null) {
                EditorGUILayout.HelpBox(
                    "No editable prefab provided.",
                    MessageType.Warning);
                return;
            }

            var instance = GetSpawnedInstance();

            using (new EditorGUILayout.HorizontalScope()) {
                GUI.enabled = instance == null;

                if (GUILayout.Button("Spawn"))
                    Spawn(prefab);

                GUI.enabled = instance != null;

                if (GUILayout.Button("Despawn"))
                    Despawn(instance);

                GUI.enabled = true;
            }

            if (instance != null) {
                EditorGUILayout.ObjectField(
                    "Spawned Instance",
                    instance,
                    typeof(GameObject),
                    true);
            }
        }
    }

    // ------------------------
    // Core logic
    // ------------------------

    void Spawn(GameObject prefab) {
        var instance = (GameObject) PrefabUtility.InstantiatePrefab(prefab, provider.transform);

        Undo.RegisterCreatedObjectUndo(instance, "Spawn Editable Prefab");

        SetSpawnedInstance(instance);

        Selection.activeGameObject = instance;
        SceneView.lastActiveSceneView?.FrameSelected();
    }

    void Despawn(GameObject instance) {
        if (instance == null)
            return;

        Undo.DestroyObjectImmediate(instance);

        SetSpawnedInstance(null);
    }

    // ------------------------
    // Spawn tracking helpers
    // ------------------------

    GameObject GetSpawnedInstance() {
        if (spawnedInstanceProp == null)
            return null;

        return spawnedInstanceProp.objectReferenceValue as GameObject;
    }

    void SetSpawnedInstance(GameObject instance) {
        if (spawnedInstanceProp == null)
            return;

        serializedObject.Update();
        spawnedInstanceProp.objectReferenceValue = instance;
        serializedObject.ApplyModifiedProperties();
    }

}
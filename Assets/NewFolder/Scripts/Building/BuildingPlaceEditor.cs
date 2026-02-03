using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(BuildingPlace))]
public class BuildingPlaceEditor : Editor {

    private void OnEnable() => EditorApplication.update += OnEditorUpdate;

    private void OnDisable() => EditorApplication.update -= OnEditorUpdate;

    private void OnEditorUpdate() {
        foreach (var target in targets)
            if (target is BuildingPlace buildingPlace)
                buildingPlace.CheckScenePreview();
    }
    
}
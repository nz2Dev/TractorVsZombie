using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MilitaryBuildingPlace))]
public class MilitaryBuildingPlaceEditor : Editor {
    
    private void OnEnable() {
        EditorApplication.update += OnEditorUpdate;
    }

    private void OnDisable() {
        EditorApplication.update -= OnEditorUpdate;
    }

    private void OnEditorUpdate() {
        var place = target as MilitaryBuildingPlace;
        if (place != null)
            place.CheckScenePreview();
    }
}
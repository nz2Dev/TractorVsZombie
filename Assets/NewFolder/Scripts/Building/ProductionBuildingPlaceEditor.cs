using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ProductionBuildingPlace))]
public class ProductionBuildingPlaceEditor : Editor {
    
    private void OnEnable() {
        EditorApplication.update += OnEditorUpdate;
    }

    private void OnDisable() {
        EditorApplication.update -= OnEditorUpdate;
    }

    private void OnEditorUpdate() {
        var place = target as ProductionBuildingPlace;
        if (place != null)
            place.CheckScenePreview();
    }
}
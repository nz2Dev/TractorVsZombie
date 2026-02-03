using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;

public enum BuildingConfigType {
    ProductionBuilding,
    HeadquarterBuilding
}

[SelectionBase]
[ExecuteInEditMode]
public class BuildingPlace : MonoBehaviour {
    
    public BuildingConfigType configType;
    public ProductionBuildingConfig productionBuildingConfig;
    public HeadquarterBuildingConfig headquarterBuildingConfig;

    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;

    private GameObject spawnedPreviewPrefabRef;

    private void Start() {
        if (Application.isPlaying)
            for (int i = 0; i < transform.childCount; i++)
                GameObject.Destroy(transform.GetChild(i).gameObject);
    }

    internal void CheckScenePreview() {
        if (Application.isPlaying)
            return;

        var configVisualsPrefab = GetBuildingTypeConfigVisualsPrefab();
        if (configVisualsPrefab == null)
            Debug.LogWarning($"No visuals prefab for {configType}: config not assigned in {name} or switch branch not implemented");

        if (spawnedPreviewPrefabRef != configVisualsPrefab) {
            for (int i = 0; i < transform.childCount; i++)
                GameObject.DestroyImmediate(transform.GetChild(i).gameObject);

            spawnedPreviewPrefabRef = configVisualsPrefab;
            if (spawnedPreviewPrefabRef != null) {
                var preview = GameObject.Instantiate(configVisualsPrefab, transform);
                preview.name += " (preview)";
            }
        }
    }

    private GameObject GetBuildingTypeConfigVisualsPrefab() {
        return configType switch {
            BuildingConfigType.ProductionBuilding => productionBuildingConfig == null ? null : productionBuildingConfig.visualsPrefab,
            BuildingConfigType.HeadquarterBuilding => headquarterBuildingConfig == null ? null : headquarterBuildingConfig.visualsPrefab,
            _ => null,
        };
    }

    public static BuildingPlace[] ScanSceneForPlaces() {
        return FindObjectsByType<BuildingPlace>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
    }

}

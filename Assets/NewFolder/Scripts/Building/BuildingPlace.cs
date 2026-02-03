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

    // Editor-only tracking
    [SerializeField, HideInInspector]
    GameObject spawnedInstance;

    // Exposed for editor
    public GameObject SpawnedInstance
    {
        get => spawnedInstance;
        set => spawnedInstance = value;
    }

    private void Start() {
        if (Application.isPlaying)
            for (int i = 0; i < transform.childCount; i++)
                GameObject.Destroy(transform.GetChild(i).gameObject);
    }

    public GameObject GetBuildingTypeConfigVisualsPrefab() {
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

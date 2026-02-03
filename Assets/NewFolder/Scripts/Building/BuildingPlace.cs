using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

using Castle.Core.Logging;

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
    public List<GameObject> spawnedInstances = new List<GameObject>();

    private void Start() {
        if (Application.isPlaying)
            for (int i = 0; i < transform.childCount; i++)
                GameObject.Destroy(transform.GetChild(i).gameObject);
    }

    public IEnumerable<GameObject> GetEditablePrefabs() {
        switch (configType) {
            case BuildingConfigType.ProductionBuilding:
                return ListProductionBuildingEditablePrefabs();
            case BuildingConfigType.HeadquarterBuilding:
                return ListHeadquarterBuildingEditablePrefabs();
            default: 
                return Array.Empty<GameObject>();
        };
    }

    private IEnumerable<GameObject> ListProductionBuildingEditablePrefabs() {
        if (productionBuildingConfig == null)
            yield break;
            
        yield return productionBuildingConfig.visualsPrefab;      
        yield return NonNullGOPrefab(productionBuildingConfig.vehicleObstaclePrefab);
    }

    private IEnumerable<GameObject> ListHeadquarterBuildingEditablePrefabs() {
        if (headquarterBuildingConfig == null)
            yield break;
        
        yield return headquarterBuildingConfig.visualsPrefab;
        yield return NonNullGOPrefab(headquarterBuildingConfig.vehicleObstaclePrefab);
    }

    private GameObject NonNullGOPrefab(MonoBehaviour monoBehavior) {
        return monoBehavior == null ? null : monoBehavior.gameObject;
    }

    public static BuildingPlace[] ScanSceneForPlaces() {
        return FindObjectsByType<BuildingPlace>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
    }

}

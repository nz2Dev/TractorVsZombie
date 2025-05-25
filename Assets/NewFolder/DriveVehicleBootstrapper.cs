using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriveVehiclesBootstrapper : MonoBehaviour {
    
    [SerializeField] private DriveVehicleEntity source;

    private DriveVehiclePhysics vehiclePhysics;
    private DriveVehicleVisuals vehicleVisuals;

    [ContextMenu("Reconstruct Nested")]
    public void Reconstruct() {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
        
        ConstructComponents(transform);
    }

    private void ConstructComponents(Transform container = null) {
        vehiclePhysics = new DriveVehiclePhysics(source);
        vehiclePhysics.Construct(container: transform);

        vehicleVisuals = new DriveVehicleVisuals(source);
        vehicleVisuals.Construct(container: transform);
    }

}

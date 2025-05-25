using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriveVehiclesBootstrapper : MonoBehaviour {
    
    [SerializeField] private DriveVehicleEntity source;

    private DriveVehiclePhysics vehiclePhysics;
    private DriveVehicleVisuals vehicleVisuals;

    private void Awake() {
        Reconstruct();
    }

    [ContextMenu("Reconstruct Nested")]
    public void Reconstruct() {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);
        
        ConstructComponents(transform);
    }

    private void Update() {
        vehiclePhysics.GetWorldPose(out var physicsPosition, out var physicsRotation);
        vehicleVisuals.SetPositionAndRotation(physicsPosition, physicsRotation);

        for (int wheelRowIndex = 0; wheelRowIndex < source.wheelRows.Length; wheelRowIndex++) {
            vehiclePhysics.GetWheelRowWorldPos(wheelRowIndex, 
                out var positionL, out var rotationL, 
                out var positionR, out var rotationR);
            vehicleVisuals.SetWheelRow(wheelRowIndex, 
                positionL, rotationL, 
                positionR, rotationR);
        }
    }

    private void ConstructComponents(Transform container = null) {
        vehiclePhysics = new DriveVehiclePhysics(source);
        vehiclePhysics.Construct(container);

        vehicleVisuals = new DriveVehicleVisuals(source);
        vehicleVisuals.Construct(container);
    }

}

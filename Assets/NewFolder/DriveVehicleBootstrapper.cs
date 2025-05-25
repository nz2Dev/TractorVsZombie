using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriveVehiclesBootstrapper : MonoBehaviour {
    
    [SerializeField] private DriveVehicleEntity source;

    [ContextMenu("Reconstruct In Scene")]
    public void Reconstruct() {
        var vehiclePhysics = new DriveVehiclePhysics(source);
        vehiclePhysics.Construct(scene: gameObject.scene);
    }

}

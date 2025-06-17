using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiclePreview : MonoBehaviour {
    [SerializeField] private VehicleEntity vehicleEntity;

    [ContextMenu("Preview")]
    private void Preview() {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);

        var vehicleService = new VehicleService(physicsContainer: transform);
        vehicleService.CreateVehicle(vehicleEntity.baseSize, vehicleEntity.wheelAxisDatas);
        
        var vehicleView = new VehicleView(container: transform);
        vehicleView.AddVehicle(Vector3.zero, vehicleEntity.baseGeometry, vehicleEntity.wheelGeometry, vehicleEntity.towingBodyGeometry, vehicleEntity.wheelAxisDatas, vehicleEntity.GetTowingWheelAxisData());
    }
}

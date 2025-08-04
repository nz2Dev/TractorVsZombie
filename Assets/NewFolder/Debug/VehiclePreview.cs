using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiclePreview : MonoBehaviour {
    [SerializeField] private VehicleBlueprint vehicleEntity;

    [ContextMenu("Preview")]
    private void Preview() {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);

        var vehicleService = new VehicleService(CreatePreviewPhysicsRoot());
        vehicleService.CreateVehicle(vehicleEntity.baseSize, vehicleEntity.wheelAxisDatas);
        
        var vehicleView = new VehicleView(container: transform);
        vehicleView.AddVehicle(Vector3.zero, vehicleEntity.baseGeometry, vehicleEntity.wheelGeometry, vehicleEntity.towingBodyGeometry, vehicleEntity.wheelAxisDatas, vehicleEntity.GetTowingWheelAxisData());
    }

    private VehiclePhysicsRoot CreatePreviewPhysicsRoot() {
        var go = new GameObject("vehicle physics root", typeof(VehiclePhysicsRoot));
        var root = go.GetComponent<VehiclePhysicsRoot>();
        root.OverrideContainer(transform);
        return root;
    }
}

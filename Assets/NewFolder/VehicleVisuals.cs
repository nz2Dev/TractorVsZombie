using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;

public class VehicleVisuals {

    struct WheelAxis {
        public GameObject leftWheel;
        public GameObject rightWheel;
    } 

    private readonly GameObject root;
    private readonly List<WheelAxis> wheelsAxes = new ();

    public VehicleVisuals(Transform container = null) {
        root = new GameObject($"Vehicle Visuals (New)");
        root.transform.SetParent(container, worldPositionStays: false);
    }

    public void AddBaseGeometry(GameObject baseGeometryPrefab, Transform baseGeometryFit) {
        var baseGeometry = Object.Instantiate(baseGeometryPrefab, root.transform, worldPositionStays: false);
        baseGeometry.transform.SetLocalPositionAndRotation(baseGeometryFit.position, baseGeometryFit.rotation);
    }

    public void AddWheelAxisGeometries(GameObject wheelGeometry, Transform wheelGeometryFit, float forwardOffset, float upOffset, float halfLength, float radius) {
        var wheelGeometryL = Object.Instantiate(wheelGeometry, root.transform, worldPositionStays: false);
        wheelGeometryL.transform.localPosition = new Vector3(-halfLength, upOffset, forwardOffset);
        wheelGeometryL.transform.localScale = wheelGeometryFit.localScale * radius;

        var wheelGeometryR = Object.Instantiate(wheelGeometry, root.transform, worldPositionStays: false);
        wheelGeometryR.transform.localPosition = new Vector3(halfLength, upOffset, forwardOffset);
        wheelGeometryR.transform.localScale = wheelGeometryFit.localScale * radius;

        wheelsAxes.Add(new WheelAxis {
            leftWheel = wheelGeometryL,
            rightWheel = wheelGeometryR
        });
    }

    public void SetPositionAndRotation(VehiclePose vehiclePose) {
        root.transform.SetPositionAndRotation(vehiclePose.position, vehiclePose.rotation);
    }

    public void SetAxisPositionAndRotation(int axisIndex, WheelAxisPose axisPose) {
        var wheelAxis = wheelsAxes[axisIndex];
        wheelAxis.leftWheel.transform.SetPositionAndRotation(axisPose.positionL, axisPose.rotationL);
        wheelAxis.rightWheel.transform.SetPositionAndRotation(axisPose.positionR, axisPose.rotationR);
    }

}

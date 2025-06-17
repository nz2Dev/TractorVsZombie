using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;

public class VehicleVisuals {

    struct WheelAxis {
        public GameObject leftWheel;
        public GameObject rightWheel;
        public GameObject towingBody;
    }

    private readonly GameObject root;
    private readonly List<WheelAxis> wheelsAxes = new ();

    public VehicleVisuals(Vector3 position, Transform container = null) {
        root = new GameObject($"Vehicle Visuals (New)");
        root.transform.SetParent(container, worldPositionStays: false);
        root.transform.position = position;
    }

    public void AddBaseGeometry(GameObject baseGeometryPrefab) {
        var baseGeometry = Object.Instantiate(baseGeometryPrefab, Vector3.zero, Quaternion.identity);
        baseGeometry.transform.SetParent(root.transform, worldPositionStays: false);
    }

    public void AddWheelAxisGeometries(GameObject wheelGeometry, float forwardOffset, float upOffset, float halfLength, float radius) {
        var wheelGeometryL = Object.Instantiate(wheelGeometry, root.transform, worldPositionStays: false);
        wheelGeometryL.transform.localPosition = new Vector3(-halfLength, upOffset, forwardOffset);
        wheelGeometryL.transform.localScale = Vector3.one * radius;

        var wheelGeometryR = Object.Instantiate(wheelGeometry, root.transform, worldPositionStays: false);
        wheelGeometryR.transform.localPosition = new Vector3(halfLength, upOffset, forwardOffset);
        wheelGeometryR.transform.localScale = Vector3.one * radius;

        wheelsAxes.Add(new WheelAxis {
            leftWheel = wheelGeometryL,
            rightWheel = wheelGeometryR
        });
    }

    internal void AddTowingWheelAxisGeometries(GameObject wheelGeometry, GameObject towingBodyGeometryPrefab, float forwardOffset, float upOffset, float halfLength, float radius, float towingBodyLength) {
        var wheelGeometryL = Object.Instantiate(wheelGeometry, root.transform, worldPositionStays: false);
        wheelGeometryL.transform.localPosition = new Vector3(-halfLength, upOffset, forwardOffset);
        wheelGeometryL.transform.localScale = Vector3.one * radius;

        var wheelGeometryR = Object.Instantiate(wheelGeometry, root.transform, worldPositionStays: false);
        wheelGeometryR.transform.localPosition = new Vector3(halfLength, upOffset, forwardOffset);
        wheelGeometryR.transform.localScale = Vector3.one * radius;

        var towingBodyGeometry = Object.Instantiate(towingBodyGeometryPrefab, root.transform, worldPositionStays: false);
        towingBodyGeometry.transform.localPosition = new Vector3(0, upOffset, forwardOffset);
        towingBodyGeometry.transform.localScale = new Vector3(1, 1, towingBodyLength);

        wheelsAxes.Add(new WheelAxis {
            leftWheel = wheelGeometryL,
            rightWheel = wheelGeometryR,
            towingBody = towingBodyGeometry
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

    internal void SetTowingAxisPositionAndRotation(TowingWheelAxisPose axisPose) {
        var wheelAxis = wheelsAxes.Single(axis => axis.towingBody != null);
        wheelAxis.leftWheel.transform.SetPositionAndRotation(axisPose.positionL, axisPose.rotationL);
        wheelAxis.rightWheel.transform.SetPositionAndRotation(axisPose.positionR, axisPose.rotationR);
        wheelAxis.towingBody.transform.rotation = axisPose.tipRotation;
    }

}

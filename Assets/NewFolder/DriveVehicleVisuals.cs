using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;

public class DriveVehicleVisuals {

    private readonly DriveVehicleEntity source;

    private GameObject gameObject;

    private GameObject[] wheelsGeometries;

    public DriveVehicleVisuals(DriveVehicleEntity source) {
        this.source = source;
    }

    public GameObject Construct(Transform container = null) {
        gameObject = new GameObject($"{source.name} (Visuals)");
        gameObject.transform.SetParent(container, worldPositionStays: false);

        var baseGeometry = Object.Instantiate(source.baseGeometry, gameObject.transform, worldPositionStays: false);
        baseGeometry.transform.SetLocalPositionAndRotation(source.baseGeometryFit.position, source.baseGeometryFit.rotation);

        int rowIndex = 0;
        wheelsGeometries = new GameObject[source.wheelRows.Length * 2];
        foreach (var wheelRow in source.wheelRows) {
            var wheelGeometryL = Object.Instantiate(source.wheelGeometry, gameObject.transform, worldPositionStays: false);
            wheelGeometryL.transform.localPosition = new Vector3(-wheelRow.rowOffset, wheelRow.verticalOffset, wheelRow.horizontalOffset);
            wheelGeometryL.transform.localScale = source.wheelGeometryFit.localScale * wheelRow.radius;

            var wheelGeometryR = Object.Instantiate(source.wheelGeometry, gameObject.transform, worldPositionStays: false);
            wheelGeometryR.transform.localPosition = new Vector3(wheelRow.rowOffset, wheelRow.verticalOffset, wheelRow.horizontalOffset);
            wheelGeometryR.transform.localScale = source.wheelGeometryFit.localScale * wheelRow.radius;

            wheelsGeometries[rowIndex * 2 + 0] = wheelGeometryL;
            wheelsGeometries[rowIndex * 2 + 1] = wheelGeometryR;
            rowIndex++;
        }
        
        return gameObject;
    }

    public void SetPositionAndRotation(Vector3 position, Quaternion rotation) {
        gameObject.transform.SetPositionAndRotation(position, rotation);
    }

    public void SetWheelRow(int rowIndex, Vector3 positionL, Quaternion rotationL, Vector3 positionR, Quaternion rotationR) {
        var wheelGeometryL = wheelsGeometries[rowIndex * 2 + 0];
        wheelGeometryL.transform.SetPositionAndRotation(positionL, rotationL);
        var wheelGeometryR = wheelsGeometries[rowIndex * 2 + 1];
        wheelGeometryR.transform.SetPositionAndRotation(positionR, rotationR);
    }

}

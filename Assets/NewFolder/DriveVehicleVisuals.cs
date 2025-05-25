using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.SceneManagement;

public class DriveVehicleVisuals {

    private readonly DriveVehicleEntity source;

    private GameObject gameObject;

    public DriveVehicleVisuals(DriveVehicleEntity source) {
        this.source = source;
    }

    public GameObject Construct(Transform container = null) {
        gameObject = new GameObject($"{source.name} (Visuals)");
        gameObject.transform.parent = container;

        var baseGeometry = Object.Instantiate(source.baseGeometry, gameObject.transform, worldPositionStays: false);
        baseGeometry.transform.SetLocalPositionAndRotation(source.baseGeometryFit.position, source.baseGeometryFit.rotation);

        foreach (var wheelRow in source.wheelRows) {
            var wheelGeometryL = Object.Instantiate(source.wheelGeometry, gameObject.transform, worldPositionStays: false);
            wheelGeometryL.transform.localPosition = new Vector3(-wheelRow.rowOffset, wheelRow.verticalOffset, wheelRow.horizontalOffset);
            wheelGeometryL.transform.localScale = source.wheelGeometryFit.localScale * wheelRow.radius;

            var wheelGeometryR = Object.Instantiate(source.wheelGeometry, gameObject.transform, worldPositionStays: false);
            wheelGeometryR.transform.localPosition = new Vector3(wheelRow.rowOffset, wheelRow.verticalOffset, wheelRow.horizontalOffset);
            wheelGeometryR.transform.localScale = source.wheelGeometryFit.localScale * wheelRow.radius;
        }
        
        return gameObject;
    }

}

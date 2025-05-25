using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class DriveVehicleVisuals : MonoBehaviour {

    [SerializeField] private DriveVehicleEntity source;

    [ContextMenu("Reconstruct")]
    public void Reconstruct() {
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);

        var baseGeometry = Instantiate(source.baseGeometry, transform, worldPositionStays: false);
        baseGeometry.transform.SetLocalPositionAndRotation(source.baseGeometryFit.position, source.baseGeometryFit.rotation);

        foreach (var wheelRow in source.wheelRows) {
            var wheelGeometryL = Instantiate(source.wheelGeometry, transform, worldPositionStays: false);
            wheelGeometryL.transform.localPosition = new Vector3(-wheelRow.rowOffset, wheelRow.verticalOffset, wheelRow.horizontalOffset);
            wheelGeometryL.transform.localScale = source.wheelGeometryFit.localScale * wheelRow.radius;

            var wheelGeometryR = Instantiate(source.wheelGeometry, transform, worldPositionStays: false);
            wheelGeometryR.transform.localPosition = new Vector3(wheelRow.rowOffset, wheelRow.verticalOffset, wheelRow.horizontalOffset);
            wheelGeometryR.transform.localScale = source.wheelGeometryFit.localScale * wheelRow.radius;
        }
    }

}

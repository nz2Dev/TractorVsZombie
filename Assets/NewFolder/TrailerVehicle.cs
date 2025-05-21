using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TrailerVehicle : System.IDisposable {

    private GameObject vehicleGO;

    public Vector3 HeadPosition =>
        vehicleGO.transform.position + vehicleGO.GetComponent<HingeJoint>().anchor;
    public Vector3 Position => vehicleGO.transform.position;

    public TrailerVehicle() {
        var trailerVehiclePrefab = Resources.Load<GameObject>("Trailer Vehicle");
        vehicleGO = Object.Instantiate(trailerVehiclePrefab);
    }

    public void SetPosition(Vector3 position) {
        vehicleGO.transform.position = position;

        var hinge = vehicleGO.GetComponent<HingeJoint>();
        if (hinge.connectedBody == null) {
            var connectedAnchorWSPosition = position + hinge.anchor;
            hinge.connectedAnchor = connectedAnchorWSPosition;
        }
    }

    public void Connect(Rigidbody rigidbody, Vector3 connectionOffset) {
        var hingeJoint = vehicleGO.GetComponent<HingeJoint>();
        hingeJoint.connectedBody = rigidbody;
        hingeJoint.connectedAnchor = connectionOffset;
        const float infiniteMassScale = 0;
        hingeJoint.connectedMassScale = infiniteMassScale;
    }

    public void Dispose() {
        Object.Destroy(vehicleGO);
    }
}

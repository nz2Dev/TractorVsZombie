using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TrailerVehicle : System.IDisposable {

    private Transform transform;
    private HingeJoint hingeJoint;

    public Vector3 Position => transform.position;
    public Vector3 HeadPosition => transform.position + hingeJoint.anchor;

    public TrailerVehicle() {
        var vehiclePrefab = Resources.Load<GameObject>("Trailer Vehicle");
        var vehicle = Object.Instantiate(vehiclePrefab);
        transform = vehicle.transform;
        hingeJoint = vehicle.GetComponent<HingeJoint>();
    }

    public void SetPosition(Vector3 position) {
        transform.position = position;

        if (hingeJoint.connectedBody == null) {
            var connectedAnchorWSPosition = position + hingeJoint.anchor;
            hingeJoint.connectedAnchor = connectedAnchorWSPosition;
        }
    }

    public void Connect(Rigidbody rigidbody, Vector3 connectionOffset) {
        hingeJoint.connectedBody = rigidbody;
        hingeJoint.connectedAnchor = connectionOffset;
        const float infiniteMassScale = 0;
        hingeJoint.connectedMassScale = infiniteMassScale;
    }

    public void Dispose() {
        Object.Destroy(transform.gameObject);
    }
}

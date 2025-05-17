using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class ConvoyMovement {

    private readonly List<GameObject> members = new();

    public void SetHeadParticipant(Vector3 position) {
        var head = InstantiateConvoyHead();
        SetupHeadVehicle(head, position);
        members.Add(head);
    }

    public void AddParticipant(Vector3 position, Vector3 size = default) {
        var newConvoyMember = InstantiateConvoyMember();
        SetupTailVehicle(newConvoyMember, position);
        ConnectWithSpringJoint(newConvoyMember, members[^1]);
        members.Add(newConvoyMember);
    }

    private void ConnectWithSpringJoint(GameObject tail, GameObject head) {
        var connectionBody = head.GetComponent<Rigidbody>();

        var connectionAnchor = head.transform.position - new Vector3(0, 0, 0.7f);
        if (head.TryGetComponent<SpringJoint>(out var headSpringJoint)) {
            var miroredAnchorVector = -headSpringJoint.anchor;
            connectionAnchor = miroredAnchorVector;
        }

        var convoyMemberJoint = tail.GetComponent<SpringJoint>();
        convoyMemberJoint.connectedBody = connectionBody;
        convoyMemberJoint.connectedAnchor = connectionAnchor;
    }

    private void SetupHeadVehicle(GameObject vehicleGO, Vector3 position) {
        const int holdBreakForce = 500;
        
        vehicleGO.transform.position = position;
        var wheelColliders = vehicleGO.GetComponentsInChildren<WheelCollider>();
        foreach (var wheelCollider in wheelColliders) {
            wheelCollider.brakeTorque = holdBreakForce;
            wheelCollider.motorTorque = 0;
        }
    }

    private void SetupTailVehicle(GameObject vehicleGO, Vector3 position) {
        vehicleGO.transform.position = position;
        var wheelColliders = vehicleGO.GetComponentsInChildren<WheelCollider>();
        foreach (var wheelCollider in wheelColliders) {
            wheelCollider.brakeTorque = 0;
            wheelCollider.motorTorque = 1;
        }
    }

    private static GameObject InstantiateConvoyHead() {
        var convoyHeadPrefab = Resources.Load<GameObject>("Convoy Head");
        return GameObject.Instantiate(convoyHeadPrefab);
    }

    private static GameObject InstantiateConvoyMember() {
        var convoyMemberPrefab = Resources.Load<GameObject>("Convoy Member");
        var convoyMember = GameObject.Instantiate(convoyMemberPrefab);
        return convoyMember;
    }

    public Vector3 GetParticipant(int index) {
        return members[index].transform.position;
    }

}
using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class ConvoyMovement {

    private GameObject head;
    private List<GameObject> members = new();

    public void SetHeadParticipant(Vector3 destination) {
        head = new GameObject();
        head.transform.position = destination;
        members.Add(head);
    }

    public void AddParticipant(Vector3 position, Vector3 size = default) {
        var newConvoyMember = InstantiateConvoyMember();
        newConvoyMember.transform.position = position;

        var connectionBody = (Rigidbody) null;
        if (members.Count > 0)
            connectionBody = members[^1].GetComponent<Rigidbody>();

        var connectionAnchor = head.transform.position;
        if (members.Count > 1)
            connectionAnchor = -members[^1].GetComponent<SpringJoint>().anchor;

        var convoyMemberJoint = newConvoyMember.GetComponent<SpringJoint>();
        convoyMemberJoint.connectedBody = connectionBody;
        convoyMemberJoint.connectedAnchor = connectionAnchor;

        var isFirstInConvoy = members.Count == 0;
        var holdBreakForce = 500;
        var wheelColliders = newConvoyMember.GetComponentsInChildren<WheelCollider>();
        foreach (var wheelCollider in wheelColliders) {
            wheelCollider.brakeTorque = isFirstInConvoy ? holdBreakForce : 0;
            wheelCollider.motorTorque = isFirstInConvoy ? 0 : 1;
        }

        members.Add(newConvoyMember);
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
using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class ConvoyMovement {

    private Vector3 anchor;
    private List<GameObject> members = new();

    public void AddParticipant(Vector3 position, Vector3 size = default) {
        var newConvoyMember = InstantiateConvoyMember();
        newConvoyMember.transform.position = position;

        var connectionBody = (Rigidbody) null;
        if (members.Count > 0)
            connectionBody = members[^1].GetComponent<Rigidbody>();

        var connectionAnchor = anchor;
        if (members.Count > 0)
            connectionAnchor = -members[^1].GetComponent<SpringJoint>().anchor;

        var convoyMemberJoint = newConvoyMember.GetComponent<SpringJoint>();
        convoyMemberJoint.connectedBody = connectionBody;
        convoyMemberJoint.connectedAnchor = connectionAnchor;

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

    public void SetDestination(Vector3 destination) {
        anchor = destination;
    }
}
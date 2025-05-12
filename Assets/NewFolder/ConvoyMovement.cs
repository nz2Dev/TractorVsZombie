using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class ConvoyMovement {

    private Rigidbody anchor;
    private List<Rigidbody> participants = new();

    public ConvoyMovement() {
        var anchorGO = new GameObject("anchor");
        anchor = anchorGO.AddComponent<Rigidbody>();
        anchor.isKinematic = true;
    }

    public void AddParticipant(Vector3 position, Vector3 size) {
        var connectionBody = anchor;
        if (participants.Count > 0) {
            connectionBody = participants[participants.Count - 1];
        }

        var convoyMemberPrefab = Resources.Load<GameObject>("Convoy Member");
        var convoyMember = GameObject.Instantiate(convoyMemberPrefab);
        var convoyMemberRigidbody = convoyMember.GetComponent<Rigidbody>();
        convoyMemberRigidbody.position = position;
        var convoyMemberJoint = convoyMember.GetComponent<SpringJoint>();
        convoyMemberJoint.connectedBody = connectionBody;
        
        participants.Add(convoyMemberRigidbody);
    }

    private Rigidbody CreateConvoyMember(Vector3 position, Rigidbody connectionBody) {
        var participantGO = new GameObject("participant" + participants.Count);
        var participantRigidbody = participantGO.AddComponent<Rigidbody>();
        participantRigidbody.position = position;
        participantRigidbody.mass = 150;

        var participantSprintJoint = participantGO.AddComponent<SpringJoint>();
        participantSprintJoint.connectedBody = connectionBody;
        participantSprintJoint.spring = 10_000;
        return participantRigidbody;
    }

    public Vector3 GetParticipant(int index) {
        return participants[index].transform.position;
    }

    public void SetDestination(Vector3 destination) {
        anchor.position = destination;
    }
}
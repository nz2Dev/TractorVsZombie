using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class ConvoyMovement {

    private readonly List<GameObject> vehicles = new();

    public void AddParticipant(Vector3 position, Quaternion rotation = default) {
        var newConvoyVehicle = InstantiateConvoyVehicle();
        SetupVehicleTransforms(newConvoyVehicle, position, rotation);

        var isFirstVehicle = vehicles.Count == 0;
        if (isFirstVehicle) {
            SetupHeadMemberWheels(newConvoyVehicle);
        } else {
            SetupTailMemberWheels(newConvoyVehicle);
            ConnectWithSpringJoint(newConvoyVehicle, vehicles[^1]);
        }

        vehicles.Add(newConvoyVehicle);
    }

    // public void UpdateTailSteerAngles() {
    //     for (int i = 1; i < vehicles.Count; i++) {
    //         var headVehicle = vehicles[i - 1];
    //         var tailVehicle = vehicles[i];

    //         var headAnchorPoint = headVehicle.transform.position - headVehicle.transform.forward * 0.7f;
    //         var tailAnchorPoint = tailVehicle.transform.position + tailVehicle.transform.forward * 0.7f;
    //         var tailToHead = headAnchorPoint - tailAnchorPoint;
    //         var flatDirectionVector = Vector3.ProjectOnPlane(tailToHead, Vector3.up);
    //         var flatForwardVector = Vector3.ProjectOnPlane(tailVehicle.transform.forward, Vector3.up);
    //         var steerAngle = Vector3.SignedAngle(flatForwardVector, flatDirectionVector.normalized, Vector3.up);
    //         // steerAngle = Math.Clamp(steerAngle, -45, 45);

    //         foreach (var wheel in tailVehicle.GetComponentsInChildren<WheelCollider>().Take(2)) {
    //             wheel.steerAngle = steerAngle;
    //             wheel.brakeTorque = 0;
    //             wheel.motorTorque = 1;
    //         }
    //     }
    // }

    private void ConnectWithSpringJoint(GameObject tail, GameObject head) {
        var anchorOffset = new Vector3(0, 0, 0.7f);
        var connectionBody = head.GetComponent<Rigidbody>();

        var tailSpringJoint = tail.AddComponent<SpringJoint>();
        tailSpringJoint.anchor = anchorOffset;
        tailSpringJoint.autoConfigureConnectedAnchor = false;
        tailSpringJoint.connectedBody = connectionBody;
        tailSpringJoint.connectedAnchor = -anchorOffset;

        tailSpringJoint.spring = 10_000;
        tailSpringJoint.damper = 3_000;
        tailSpringJoint.maxDistance = 0.2f;
    }

    private void SetupVehicleTransforms(GameObject vehicleGO, Vector3 position, Quaternion rotaiton) {
        vehicleGO.transform.SetPositionAndRotation(position, rotaiton);
    }

    private void SetupHeadMemberWheels(GameObject vehicleGO) {
        const int holdBreakForce = 500;
        var wheelColliders = vehicleGO.GetComponentsInChildren<WheelCollider>();
        foreach (var wheelCollider in wheelColliders) {
            wheelCollider.brakeTorque = holdBreakForce;
            wheelCollider.motorTorque = 0;
        }
    }

    private void SetupTailMemberWheels(GameObject vehicleGO) {
        var wheelColliders = vehicleGO.GetComponentsInChildren<WheelCollider>();
        foreach (var wheelCollider in wheelColliders) {
            wheelCollider.brakeTorque = 0;
            wheelCollider.motorTorque = 1;
        }
    }

    private static GameObject InstantiateConvoyVehicle() {
        var convoyMemberPrefab = Resources.Load<GameObject>("Convoy Vehicle");
        var convoyMember = GameObject.Instantiate(convoyMemberPrefab);
        return convoyMember;
    }

    public Vector3 GetParticipant(int index) {
        return vehicles[index].transform.position;
    }

    public Quaternion GetParticipantRotation(int index) {
        return vehicles[index].transform.rotation;
    }

    public GameObject GetParticipantGO(int index) {
        return vehicles[index];
    }

}
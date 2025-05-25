using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class DriveVehiclePhysics : MonoBehaviour {
    
    [SerializeField] private DriveVehicleEntity source;

    [ContextMenu("Reconstruct")]
    public void Reconstruct() {
        DestroyImmediate(GetComponent<Rigidbody>());        
        var rigidbody = gameObject.AddComponent<Rigidbody>();
        rigidbody.hideFlags = HideFlags.NotEditable;
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
        rigidbody.mass = 150;

        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);

        Instantiate(source.baseCollider, transform, worldPositionStays: false);
        foreach (var wheelRow in source.wheelRows) {
            var wheelL = CreateDefaultWheel(wheelRow.radius);
            wheelL.transform.localPosition = new Vector3(-wheelRow.rowOffset, wheelRow.verticalOffset, wheelRow.horizontalOffset);
            var wheelR = CreateDefaultWheel(wheelRow.radius);
            wheelR.transform.localPosition = new Vector3(wheelRow.rowOffset, wheelRow.verticalOffset, wheelRow.horizontalOffset);
        }
    }

    private GameObject CreateDefaultWheel(float radius) {
        var wheel = new GameObject("Default Wheel (New)", typeof(WheelCollider));
        wheel.transform.parent = transform; 
        var wheelCollider = wheel.GetComponent<WheelCollider>();
        SetupDefaultWheelCollider(wheelCollider, radius);
        return wheel;
    }

    private void SetupDefaultWheelCollider(WheelCollider wheelCollider, float wheelRadius) {
        wheelCollider.suspensionSpring = CreateDefaultJointSpring();
        wheelCollider.suspensionDistance = wheelRadius * 0.6f;
        wheelCollider.radius = wheelRadius;
        wheelCollider.mass = 5;
    }

    private JointSpring CreateDefaultJointSpring() {
        return new JointSpring {
            targetPosition = .5f,
            spring = 35_000,
            damper = 450,
        };
    }
}

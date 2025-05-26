using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class VehiclePhysics {
    
    private readonly VehicleEntity source;

    private GameObject gameObject;
    private WheelCollider[] wheelColliders = new WheelCollider[0];

    public VehiclePhysics(VehicleEntity source) {
        this.source = source;
    }

    public GameObject Construct(Transform container = null) {
        gameObject = new GameObject($"{source.name} (Physics)", typeof(Rigidbody));
        gameObject.transform.SetParent(container, worldPositionStays: false);

        var rigidbody = gameObject.GetComponent<Rigidbody>();
        rigidbody.hideFlags = HideFlags.NotEditable;
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
        rigidbody.mass = 150;
        
        Object.Instantiate(source.baseCollider, gameObject.transform, worldPositionStays: false);

        int rowIndex = 0;
        wheelColliders = new WheelCollider[source.wheelRows.Length * 2];
        foreach (var wheelRow in source.wheelRows) {
            var wheelL = CreateDefaultWheel(wheelRow.radius);
            wheelL.transform.localPosition = new Vector3(-wheelRow.rowOffset, wheelRow.verticalOffset, wheelRow.horizontalOffset);
            var wheelR = CreateDefaultWheel(wheelRow.radius);
            wheelR.transform.localPosition = new Vector3(wheelRow.rowOffset, wheelRow.verticalOffset, wheelRow.horizontalOffset);

            wheelColliders[rowIndex * 2 + 0] = wheelL.GetComponent<WheelCollider>();
            wheelColliders[rowIndex * 2 + 1] = wheelR.GetComponent<WheelCollider>();
            rowIndex++;
        }

        return gameObject;
    }

    public void GasThrottle(float v) {
        const float maxTorque = 400;
        for (int row = 0; row < source.wheelRows.Length; row++) {
            var wheelRowInfo = source.wheelRows[row];
            if (wheelRowInfo.drive) {
                var colliderL = wheelColliders[row * 2 + 0];
                colliderL.motorTorque = v * maxTorque;
                colliderL.brakeTorque = 0;
                var colliderR = wheelColliders[row * 2 + 1];
                colliderR.motorTorque = v * maxTorque;
                colliderR.brakeTorque = 0;
            }            
        }
    }

    public void GetWorldPose(out Vector3 position, out Quaternion rotation) {
        position = gameObject.transform.position;
        rotation = gameObject.transform.rotation;
    }

    public void GetWheelRowWorldPos(int rowIndex, out Vector3 positionL, out Quaternion rotationL, out Vector3 positionR, out Quaternion rotationR) {
        var leftWheel = wheelColliders[rowIndex * 2 + 0];
        leftWheel.GetWorldPose(out positionL, out rotationL);
        var rightWheel = wheelColliders[rowIndex * 2 + 1];
        rightWheel.GetWorldPose(out positionR, out rotationR);
    }

    private GameObject CreateDefaultWheel(float radius) {
        var wheel = new GameObject("Default Wheel (New)", typeof(WheelCollider));
        wheel.transform.hideFlags = HideFlags.NotEditable;
        wheel.transform.parent = gameObject.transform; 
        var wheelCollider = wheel.GetComponent<WheelCollider>();
        wheelCollider.hideFlags = HideFlags.NotEditable;
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

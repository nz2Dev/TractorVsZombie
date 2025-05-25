using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

public class DriveVehiclePhysics {
    
    private readonly DriveVehicleEntity source;

    private GameObject gameObject;

    public DriveVehiclePhysics(DriveVehicleEntity source) {
        this.source = source;
    }

    public void Construct(Scene scene) {
        gameObject = new GameObject($"{source.name} (Physics)", typeof(Rigidbody));
        SceneManager.MoveGameObjectToScene(gameObject, scene);

        var rigidbody = gameObject.GetComponent<Rigidbody>();
        rigidbody.hideFlags = HideFlags.NotEditable;
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
        rigidbody.mass = 150;
        
        Object.Instantiate(source.baseCollider, gameObject.transform, worldPositionStays: false);
        foreach (var wheelRow in source.wheelRows) {
            var wheelL = CreateDefaultWheel(wheelRow.radius);
            wheelL.transform.localPosition = new Vector3(-wheelRow.rowOffset, wheelRow.verticalOffset, wheelRow.horizontalOffset);
            var wheelR = CreateDefaultWheel(wheelRow.radius);
            wheelR.transform.localPosition = new Vector3(wheelRow.rowOffset, wheelRow.verticalOffset, wheelRow.horizontalOffset);
        }
    }

    public void DestroyConstruction() {
        Object.Destroy(gameObject);
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

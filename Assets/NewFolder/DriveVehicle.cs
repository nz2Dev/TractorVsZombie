
using System;

using UnityEngine;

public class DriveVehicle : IDisposable {

    private readonly Transform transform;
    private readonly Rigidbody rigidbody;
    private readonly WheelCollider[] wheels;

    private WheelCollider BackWheelL => wheels[0];
    private WheelCollider BackWheelR => wheels[1];
    private WheelCollider FrontWheelL => wheels[2];
    private WheelCollider FrontWheelR => wheels[3];

    public DriveVehicle() {
        var driveVehiclePrefab = Resources.Load<GameObject>("Drive Vehicle");

        var vehicleGO = UnityEngine.Object.Instantiate(driveVehiclePrefab);
        transform = vehicleGO.transform;
        rigidbody = vehicleGO.GetComponent<Rigidbody>();
        wheels = vehicleGO.GetComponentsInChildren<WheelCollider>();
    }

    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;
    public Rigidbody Rigidbody => rigidbody;

    public void Brakes(float breakTorque) {
        foreach (var wheel in wheels)
            wheel.brakeTorque = breakTorque;
    }

    public void Gas(float motorTorque) {
        foreach (var wheel in wheels)
            wheel.motorTorque = motorTorque;
    }

    public void Steer(float angleDegrees) {
        FrontWheelL.steerAngle = angleDegrees;
        FrontWheelR.steerAngle = angleDegrees;
    }

    public void AddForce(Vector3 force) {
        rigidbody.AddForce(force, ForceMode.Force);
    }

    public void Dispose() {
        UnityEngine.Object.Destroy(transform.gameObject);
    }

}
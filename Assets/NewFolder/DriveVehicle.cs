
using System;

using UnityEngine;

public class DriveVehicle : IDisposable {

    private readonly float maxTorque = 150f;

    private readonly Transform transform;
    private readonly Rigidbody rigidbody;
    private readonly WheelCollider[] wheels;

    public WheelCollider BackWheelL => wheels[0];
    public WheelCollider BackWheelR => wheels[1];
    public WheelCollider FrontWheelL => wheels[2];
    public WheelCollider FrontWheelR => wheels[3];

    public DriveVehicle(float maxTorque = 150) {
        this.maxTorque = maxTorque;

        var driveVehiclePrefab = Resources.Load<GameObject>("Drive Vehicle");
        var vehicleGO = UnityEngine.Object.Instantiate(driveVehiclePrefab);
        transform = vehicleGO.transform;
        rigidbody = vehicleGO.GetComponent<Rigidbody>();
        wheels = vehicleGO.GetComponentsInChildren<WheelCollider>();
    }

    public Vector3 Position => transform.position;
    public Quaternion Rotation => transform.rotation;
    public Rigidbody Rigidbody => rigidbody;
    public float Speed => rigidbody.velocity.magnitude;

    public void Brakes(float brakesThrottle) {
        foreach (var wheel in wheels)
            wheel.brakeTorque = brakesThrottle * maxTorque;
    }

    public void Throttle(float throttle) {
        throttle = Mathf.Clamp01(throttle);
        foreach (var wheel in wheels)
            wheel.motorTorque = throttle * maxTorque;
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
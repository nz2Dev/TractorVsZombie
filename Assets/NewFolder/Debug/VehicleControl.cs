using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public struct WheelAxis {
    public WheelCollider wheelL;
    public WheelCollider wheelR;
    public float motorTorque;
    public float breakTorque;
    public float steerAngle;
}

public class VehicleControl : MonoBehaviour {

    [SerializeField] private WheelAxis[] axes;

    private void FixedUpdate() {
        foreach (var axis in axes) {
            UpdateWheel(axis.wheelL, axis.motorTorque, axis.breakTorque, axis.steerAngle);
            UpdateWheel(axis.wheelR, axis.motorTorque, axis.breakTorque, axis.steerAngle);
        }
    }

    private void UpdateWheel(WheelCollider wheel, float motorTorque, float breakTorque, float steerAngle) {
        wheel.motorTorque = motorTorque;
        wheel.brakeTorque = breakTorque;
        wheel.steerAngle = steerAngle;
    }
}

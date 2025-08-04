using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelDebug : MonoBehaviour {
    
    public float motorTorque;
    public float breakTorque;
    public float steerAngle;

    private WheelCollider wheelCollider;


    private void Awake() {
        wheelCollider = GetComponent<WheelCollider>();
    }

    private void FixedUpdate() {
        UpdateVariables();
    }

    private void Update() {
        UpdateVariables();
    }

    private void UpdateVariables() {
        motorTorque = wheelCollider.motorTorque;
        breakTorque = wheelCollider.brakeTorque;
        steerAngle = wheelCollider.steerAngle;
    }
}

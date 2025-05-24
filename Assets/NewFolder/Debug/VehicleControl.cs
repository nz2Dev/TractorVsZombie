using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class VehicleControl : MonoBehaviour {

    [SerializeField] private float forwardWheelsMotorTorque = 0;
    [SerializeField] private float rearWheelsMotorTorque = 0;
    [SerializeField] private float breakTorque = 0;

    [SerializeField] private WheelCollider[] rearWheels;
    [SerializeField] private WheelCollider[] forwardWheels;

    private void FixedUpdate() {
        foreach (var wheel in rearWheels) {
            wheel.motorTorque = rearWheelsMotorTorque;
            wheel.brakeTorque = breakTorque;
        }
        
        foreach (var wheel in forwardWheels) {
            wheel.motorTorque = forwardWheelsMotorTorque;
            wheel.brakeTorque = breakTorque;
        }
    }
}

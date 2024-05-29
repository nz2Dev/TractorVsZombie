using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargoControl : MonoBehaviour {
    [SerializeField] private Transform carier;
    [SerializeField] private float breakTorque = 0;
    [SerializeField] private float motorTorque = 0;

    private WheelControl[] wheels;

    private void Start() {
        wheels = GetComponentsInChildren<WheelControl>();
    }

    private void Update() {
        var cargoToCarrier = carier.transform.position - transform.position;
        var flatDirectionVector = Vector3.ProjectOnPlane(cargoToCarrier, Vector3.up);
        var flatForwardVector = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        var steerAngle = Vector3.SignedAngle(flatForwardVector, flatDirectionVector.normalized, Vector3.up);
        foreach (var wheel in wheels) {
            if (wheel.steerable) {
                wheel.WheelCollider.steerAngle = steerAngle;
            }
        }

        foreach (var wheel in wheels) {
            wheel.WheelCollider.brakeTorque = breakTorque;
            wheel.WheelCollider.motorTorque = motorTorque;
        }
    }
}

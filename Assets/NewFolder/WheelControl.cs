using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelControl : MonoBehaviour {

    [SerializeField] private float breakTorque = 0;
    [SerializeField] private float motorTorque = 0;
    [SerializeField] private float steerAngle = 0;
    [Space]
    [SerializeField] private Transform steerTowards;

    private void Update() {
        var wheelCollider = GetComponent<WheelCollider>();
        wheelCollider.brakeTorque = breakTorque;
        wheelCollider.motorTorque = motorTorque;

        if (steerTowards != null) {
            var wheelToTarget = steerTowards.position - transform.position;
            var flatDirectionVector = Vector3.ProjectOnPlane(wheelToTarget, Vector3.up);
            var flatForwardVector = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
            var steerAngle = Vector3.SignedAngle(flatForwardVector, flatDirectionVector.normalized, Vector3.up);
            wheelCollider.steerAngle = steerAngle;
            this.steerAngle = steerAngle;
        }
        else {
            wheelCollider.steerAngle = steerAngle;
        }
    }

}

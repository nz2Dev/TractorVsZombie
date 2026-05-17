using System;

using UnityEditor;

using UnityEngine;

public enum WheelAxisName {
    Front,
    Rear
}

[Serializable]
public struct WheelAxis {
    public float depth;
    public float width;
    [Inline] public WheelCollider leftWheel;
    [Inline] public WheelCollider rightWheel;
}

public class VehicleChassie : MonoBehaviour {

    [SerializeField] internal WheelAxis frontAxis;
    [SerializeField] internal WheelAxis rearAxis;

#if UNITY_EDITOR
    private void OnValidate() {
        AdjustAxisWheels(frontAxis);
        AdjustAxisWheels(rearAxis);
    }
#endif

    public void SetAxisfMotorTorque(WheelAxisName name, float torque) {
        var wheelAxis = GetAxisByName(name);
        wheelAxis.leftWheel.motorTorque = torque;
        wheelAxis.rightWheel.motorTorque = torque;
    }

    internal void SetAxisSteerAngle(WheelAxisName axisName, float steeringDegree) {
        var wheelAxis = GetAxisByName(axisName);
        wheelAxis.leftWheel.steerAngle = steeringDegree;
        wheelAxis.rightWheel.steerAngle = steeringDegree;
    }

    private void AdjustAxisWheels(WheelAxis axis) {
        var leftWheelRadius = axis.leftWheel.radius;
        axis.leftWheel.transform.localPosition = new Vector3(-0.5f * axis.width, leftWheelRadius, axis.depth);
        
        var rightWheelRadius = axis.rightWheel.radius;
        axis.rightWheel.transform.localPosition = new Vector3(0.5f * axis.width, rightWheelRadius, axis.depth);
    }

    private WheelAxis GetAxisByName(WheelAxisName name) {
        return name == WheelAxisName.Front ? frontAxis : name == WheelAxisName.Rear ? rearAxis : throw new Exception($"{name}");
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        DrawAxisBoundary(frontAxis);
        DrawAxisBoundary(rearAxis);
    }

    private void DrawAxisBoundary(WheelAxis axis) {
        var left = new Vector3(-0.5f * axis.width, 0, axis.depth);
        var right = new Vector3(0.5f * axis.width, 0, axis.depth);
        Handles.matrix = transform.localToWorldMatrix;
        Handles.DrawLine(left, right, 2f);
    }
#endif
}
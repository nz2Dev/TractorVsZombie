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

    public readonly bool AreWheelsSet() {
        return leftWheel != null && rightWheel != null;
    }
}

[ExecuteInEditMode]
[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class VehicleChassie : MonoBehaviour, IVehiclePullingConnectorProvider {

    [SerializeField, Inline, Local] internal BoxCollider baseCollider;
    [SerializeField] internal WheelAxis frontAxis;
    [SerializeField] internal WheelAxis rearAxis;
    
    [SerializeField, HideInInspector] Rigidbody physics;

#if UNITY_EDITOR
    private void OnValidate() {
        if (physics == null)
            physics = GetComponent<Rigidbody>();

        if (frontAxis.AreWheelsSet() && rearAxis.AreWheelsSet())
            AdjustAxes();
    }
#endif

    private void Awake() {
        if (physics == null || baseCollider == null || !frontAxis.AreWheelsSet() || !rearAxis.AreWheelsSet())
            throw new InvalidOperationException();
        
        AdjustAxes();
    }

    internal void SetAxisfMotorTorque(WheelAxisName name, float torque) {
        var wheelAxis = GetAxisByName(name);
        wheelAxis.leftWheel.motorTorque = torque;
        wheelAxis.rightWheel.motorTorque = torque;
    }

    internal void SetAxisSteerAngle(WheelAxisName axisName, float steeringDegree) {
        var wheelAxis = GetAxisByName(axisName);
        wheelAxis.leftWheel.steerAngle = steeringDegree;
        wheelAxis.rightWheel.steerAngle = steeringDegree;
    }

    public VehiclePullingConnector GetPullingConnector() {
        return new VehiclePullingConnector {
            rigidbody = physics,
            anchorOffsetLocalSpace = new Vector3(0, 0, -0.5f * baseCollider.size.z),
        };
    }

    public void GetAxisWheels(WheelAxisName axisName, out Vector3 lPos, out Quaternion lRot, out Vector3 rPos, out Quaternion rRot) {
        var axis = GetAxisByName(axisName);
        axis.leftWheel.GetWorldPose(out lPos, out lRot);
        axis.rightWheel.GetWorldPose(out rPos, out rRot);
    }

    private void AdjustAxes() {
        AdjustAxisWheels(frontAxis);
        AdjustAxisWheels(rearAxis);
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
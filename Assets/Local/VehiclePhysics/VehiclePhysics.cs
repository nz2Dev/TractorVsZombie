using System;
using System.Runtime.InteropServices.WindowsRuntime;

using UnityEditor;

using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;

[ExecuteInEditMode]
[RequireComponent(typeof(Rigidbody))]
public class VehiclePhysics : MonoBehaviour {

    [Serializable]
    public struct WheelAxis {
        public WheelCollider leftWheel;    
        public WheelCollider rightWheel;

        public readonly bool IsCreated() {
            return leftWheel != null && rightWheel != null;
        }

        internal readonly float AnyWheelRadius() {
            return leftWheel.radius;
        }
    }

    private Rigidbody rootRigidbody;

    [SerializeField] private BoxCollider baseCollider;
    [SerializeField] private WheelAxis frontAxis;
    [SerializeField] private WheelAxis rearAxis;

    internal BoxCollider BaseCollider => baseCollider;
    internal WheelAxis FrontAxis => frontAxis;
    internal WheelAxis RearAxis => rearAxis;

    private void Awake() {
        rootRigidbody = GetComponent<Rigidbody>();
    }

    public void SetFrontAxis(WheelCollider leftWheel, WheelCollider rightWheel) {
        frontAxis = new WheelAxis {
            leftWheel = leftWheel,
            rightWheel = rightWheel
        };
    }

    public void SetRearAxis(WheelCollider leftWheel, WheelCollider rightWheel) {
        rearAxis = new WheelAxis {
            leftWheel = leftWheel,
            rightWheel = rightWheel
        };
    }

    public void SetBaseCollider(BoxCollider boxCollider) {
        baseCollider = boxCollider;
    }

    internal bool IsComponentsSet() {
        return frontAxis.IsCreated() && rearAxis.IsCreated();
    }

    public void CalculateCenterOfMass() {
        var maxAxisWheelRadius = Mathf.Max(frontAxis.AnyWheelRadius(), rearAxis.AnyWheelRadius());
        rootRigidbody.centerOfMass = new Vector3(0, -maxAxisWheelRadius * 1.5f, 0);
    }
    
    private void OnDrawGizmosSelected() {
        if (!IsComponentsSet())
            return;

        DrawAxis(frontAxis);
        DrawAxis(rearAxis);
    }

    private void DrawAxis(WheelAxis axis) {
        DrawWheel(axis.leftWheel);
        DrawWheel(axis.rightWheel);
    }

    private void DrawWheel(WheelCollider collider) {
        Handles.DrawWireDisc(collider.transform.position, collider.transform.right, collider.radius);
    }

}
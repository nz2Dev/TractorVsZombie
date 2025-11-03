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

        internal float FindFrontOffset() {
            return leftWheel.transform.localPosition.z;
        }

        internal float FindUpOffset() {
            return leftWheel.transform.localPosition.y;
        }

        internal readonly float AnyWheelRadius() {
            return leftWheel.radius;
        }
    }

    private Rigidbody rootRigidbody;

    [SerializeField] private BoxCollider baseCollider;
    [SerializeField] private WheelAxis frontAxis;
    [SerializeField] private bool towingFrontAxis;
    [SerializeField] private float towingBodyLength = 1f;
    [SerializeField] private WheelAxis rearAxis;
    [Space]
    [SerializeField] private ConfigurableJoint turningBodyJoint;
    [SerializeField] private Rigidbody turningRigidbody;

    internal BoxCollider BaseCollider => baseCollider;
    internal WheelAxis FrontAxis => frontAxis;
    internal WheelAxis RearAxis => rearAxis;

    private void Awake() {
        rootRigidbody = GetComponent<Rigidbody>();
    }

    [ContextMenu("Update Structural Changes")]
    private void UpdateStructuralChanges() {
        if (IsComponentsSet()) {
            if (towingFrontAxis) {
                if (turningRigidbody == null) {
                    SetDefaultTurningBody();
                    UpdateTurningBodyDimensions(frontAxis.FindUpOffset(), frontAxis.FindFrontOffset(), towingBodyLength);
                }
                if (turningBodyJoint == null) {
                    JointTurningBody();
                    UpdateTurningBodyJointAnchors();
                }
            } else {
                DestroyImmediate(turningBodyJoint);
                turningBodyJoint = null;
                DestroyImmediate(turningRigidbody.gameObject);
                turningRigidbody = null;
            }
        }
    }

    private void OnValidate() {
        if (IsComponentsSet()) {
            if (turningRigidbody != null && turningBodyJoint != null) {
                UpdateTurningBodyDimensions(frontAxis.FindUpOffset(), frontAxis.FindFrontOffset(), towingBodyLength);
                UpdateTurningBodyJointAnchors();
            }
        }
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

    private void SetDefaultTurningBody() {
        var turningBodyGO = new GameObject("Turning Body (New)", typeof(Rigidbody), typeof(BoxCollider));
        turningBodyGO.layer = gameObject.layer;
        turningBodyGO.transform.SetParent(transform, worldPositionStays: false);

        turningRigidbody = turningBodyGO.GetComponent<Rigidbody>();
        turningRigidbody.mass = 1;
    }

    private void UpdateTurningBodyDimensions(float upOffset, float forwardOffset, float length) {
        turningRigidbody.transform.localPosition = new Vector3(0, upOffset, forwardOffset);
        var collider = turningRigidbody.GetComponent<BoxCollider>();
        collider.center = new Vector3(0, 0, length * 0.5f);
        collider.size = new Vector3(0.025f, 0.025f, length);
    }

    private void JointTurningBody() {
        var joint = gameObject.AddComponent<ConfigurableJoint>();
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
        joint.angularXMotion = ConfigurableJointMotion.Limited;
        joint.highAngularXLimit = new SoftJointLimit { limit = 20 };
        joint.lowAngularXLimit = new SoftJointLimit { limit = -20 };
        joint.angularYMotion = ConfigurableJointMotion.Limited;
        joint.angularYLimit = new SoftJointLimit { limit = 120 };
        joint.angularZMotion = ConfigurableJointMotion.Locked;
        turningBodyJoint = joint;
    }

    private void UpdateTurningBodyJointAnchors() {
        var joint = turningBodyJoint;
        joint.anchor = transform.InverseTransformPoint(turningRigidbody.transform.position);
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedBody = turningRigidbody;
        joint.connectedAnchor = Vector3.zero;
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
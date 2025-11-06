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

    public struct VehicleConnector {
        public Rigidbody rigidbody;
        public Vector3 anchorOffset;
        public Vector3 worldAnchorRestPoint;
    }

    private Rigidbody rootRigidbody;

    [SerializeField] private BoxCollider baseCollider;
    [SerializeField] private WheelAxis frontAxis;
    [SerializeField] private WheelAxis rearAxis;
    [Space]
    [SerializeField] private ConfigurableJoint turningBodyJoint;
    [SerializeField] private Rigidbody turningRigidbody;
    [SerializeField] private BoxCollider turningBoxCollider;

    internal BoxCollider BaseCollider => baseCollider;
    internal WheelAxis FrontAxis => frontAxis;
    internal WheelAxis RearAxis => rearAxis;
    internal BoxCollider TurningBodyCollider => turningBoxCollider;

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

    public void SetTurningBody(GameObject turningBody) {
        turningBoxCollider = turningBody.GetComponent<BoxCollider>();
        turningRigidbody = turningBody.GetComponent<Rigidbody>();
        if (turningBodyJoint == null) {
            AddTurningBodyJoint();
        }
    }

    public void OnStrcutureChanged() {
        if (turningRigidbody == null && turningBodyJoint != null) {
            DestroyImmediate(turningBodyJoint);
            turningBodyJoint = null;
        }
    }

    public void OnComponentsChanged() {
        CalculateCenterOfMass();
        if (turningRigidbody != null) {
            UpdateTurningBodyPlacement();
        }
    }

    public VehicleConnector GetTowingConnector() {
        if (turningRigidbody != null) {
            return GetTurningBodyTowingConnector();
        } else {
            return GetBaseVehicleTowingConnector();
        }
    }

    private VehicleConnector GetBaseVehicleTowingConnector() {
        var baseSize = baseCollider.size;
        
        var inFrontOfBoxCollider = new Vector3(0, 0, baseSize.z * 0.5f);
        var worldAnchorRestPoint = transform.TransformPoint(inFrontOfBoxCollider);
        
        return new VehicleConnector {
            rigidbody = rootRigidbody,
            anchorOffset = inFrontOfBoxCollider,
            worldAnchorRestPoint = worldAnchorRestPoint,
        };
    }

    private VehicleConnector GetTurningBodyTowingConnector() {
        var inFrontOfTurningBodyCollider = new Vector3(0, 0, turningBoxCollider.size.z);
        var wheelAxisCenter = new Vector3(0, frontAxis.FindUpOffset(), frontAxis.FindFrontOffset());
        var worldAnchorRestPoint = transform.TransformPoint(wheelAxisCenter + inFrontOfTurningBodyCollider);

        return new VehicleConnector {
            rigidbody = turningRigidbody,
            anchorOffset = inFrontOfTurningBodyCollider,
            worldAnchorRestPoint = worldAnchorRestPoint,
        };
    }

    public VehicleConnector GetPullingConnector() {
        var baseSize = baseCollider.size;
        
        var inBackOfBoxCollider = new Vector3(0, 0, -baseSize.z * 0.5f);
        var worldAnchorRestPoint = transform.TransformPoint(inBackOfBoxCollider);
        
        return new VehicleConnector {
            rigidbody = rootRigidbody,
            anchorOffset = inBackOfBoxCollider,
            worldAnchorRestPoint = worldAnchorRestPoint,
        };
    }

    internal bool IsComponentsSet() {
        return frontAxis.IsCreated() && rearAxis.IsCreated();
    }

    public void CalculateCenterOfMass() {
        var maxAxisWheelRadius = Mathf.Max(frontAxis.AnyWheelRadius(), rearAxis.AnyWheelRadius());
        rootRigidbody.centerOfMass = new Vector3(0, -maxAxisWheelRadius * 1.5f, 0);
    }

    private void AddTurningBodyJoint() {
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
        joint.autoConfigureConnectedAnchor = false;
        turningBodyJoint = joint;
    }

    private void UpdateTurningBodyPlacement() {
        var upOffset = frontAxis.FindUpOffset();
        var forwardOffset = frontAxis.FindFrontOffset();
        turningRigidbody.transform.localPosition = new Vector3(0, upOffset, forwardOffset);

        var joint = turningBodyJoint;
        joint.anchor = transform.InverseTransformPoint(turningRigidbody.transform.position);
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
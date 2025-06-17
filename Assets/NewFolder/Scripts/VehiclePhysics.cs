using System;
using System.Collections.Generic;
using System.Linq;

using UnityEditor.Experimental.GraphView;

using UnityEngine;
using UnityEngine.Assertions;

public class VehiclePhysics {

    struct WheelAxis {
        public WheelCollider leftWheel;    
        public WheelCollider rightWheel;    
        public Rigidbody turningBody;
        public bool drive;
        public bool steer;
    }

    public struct VehicleConnector {
        public Rigidbody rigidbody;
        public Vector3 anchorOffset;
        public Vector3 worldAnchorRestPoint;
    }

    private static readonly HideFlags DefaultHideFlag = HideFlags.None;
    
    private readonly GameObject root;
    private readonly List<WheelAxis> wheelAxes = new ();

    public int AxisCount => wheelAxes.Count;
    public Vector3 Position => root.transform.position;
    public Quaternion Rotation => root.transform.rotation;

    public VehiclePhysics(Vector3 position, Transform container, float mass) {
        root = new GameObject("Vehicle Physics (New)", typeof(Rigidbody));
        root.transform.SetParent(container, worldPositionStays: false);
        root.transform.position = position;
        var rigidbody = root.GetComponent<Rigidbody>();
        rigidbody.position = position;
        rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        rigidbody.automaticCenterOfMass = false;
        rigidbody.centerOfMass = Vector3.zero;
        rigidbody.hideFlags = DefaultHideFlag;
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
        rigidbody.mass = mass;
    }

    public void ConfigureBase(Vector3 baseSize) {
        Assert.IsNull(root.GetComponentInChildren<BoxCollider>());
        var baseGameObject = new GameObject("Base Box Collider (New)", typeof(BoxCollider));
        baseGameObject.transform.SetParent(root.transform, worldPositionStays: false);
        var baseCollider = baseGameObject.GetComponent<BoxCollider>();
        baseCollider.center = new Vector3(0, baseSize.y * 0.5f, 0);
        baseCollider.size = baseSize;
    }

    public void CreateWheelAxis(float length, float upOffset, float forwardOffset, float radius, bool drive, bool steer) {
        var wheelL = CreateDefaultWheel(radius);
        wheelL.transform.localPosition = new Vector3(-length / 2f, upOffset, forwardOffset);
        var wheelR = CreateDefaultWheel(radius);
        wheelR.transform.localPosition = new Vector3(+length / 2f, upOffset, forwardOffset);
        wheelAxes.Add(new WheelAxis {
            leftWheel = wheelL,
            rightWheel = wheelR,
            drive = drive,
            steer = steer
        });

        var rigidbody = root.GetComponent<Rigidbody>();
        rigidbody.centerOfMass = new Vector3(0, -radius * 1.5f, 0);
    }

    public void CreateTowingWheelAxis(float axisLength, float upOffset, float forwardOffset, float radius, float hingeLength) {
        var wheelL = CreateDefaultWheel(radius);
        wheelL.transform.localPosition = new Vector3(-axisLength / 2f, upOffset, forwardOffset);
        var wheelR = CreateDefaultWheel(radius);
        wheelR.transform.localPosition = new Vector3(+axisLength / 2f, upOffset, forwardOffset);
        var turningBody = CreateDefaultTurningBody(upOffset - (/*half of suspension distance*/0.05f), forwardOffset, hingeLength);
        
        JointTurningBody(turningBody);
        wheelAxes.Add(new WheelAxis {
            leftWheel = wheelL,
            rightWheel = wheelR,
            turningBody = turningBody,
        });

        var rigidbody = root.GetComponent<Rigidbody>();
        rigidbody.centerOfMass = new Vector3(0, -radius * 1.5f, 0);
    }

    public void UpdateTowingWheelAxis() {
        var rigidbodyForward = root.GetComponent<Rigidbody>().rotation * Vector3.forward;
        foreach (var axis in wheelAxes) {
            if (axis.turningBody != null) {
                var turningBodyForward = axis.turningBody.rotation * Vector3.forward;
                var axisAngle = Vector3.SignedAngle(rigidbodyForward, turningBodyForward, Vector3.up);
                axis.leftWheel.steerAngle = axisAngle;
                axis.rightWheel.steerAngle = axisAngle;
            }
        }
    }

    private void JointTurningBody(Rigidbody turningBody) {
        Assert.IsNull(root.GetComponent<ConfigurableJoint>());
        var joint = root.AddComponent<ConfigurableJoint>();
        joint.hideFlags = DefaultHideFlag;
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
        joint.angularXMotion = ConfigurableJointMotion.Limited;
        joint.highAngularXLimit = new SoftJointLimit { limit = 20 };
        joint.lowAngularXLimit = new SoftJointLimit { limit = -20 };
        joint.angularYMotion = ConfigurableJointMotion.Limited;
        joint.angularYLimit = new SoftJointLimit { limit = 120 };
        joint.angularZMotion = ConfigurableJointMotion.Locked;
        joint.anchor = root.transform.InverseTransformPoint(turningBody.transform.position);
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedBody = turningBody;
        joint.connectedAnchor = Vector3.zero;
    }

    public VehicleConnector GetTowingConnector() {
        var towingAxisCount = wheelAxes.Count(axis => axis.turningBody != null);
        Assert.IsTrue(towingAxisCount == 1 || towingAxisCount == 0);
        if (towingAxisCount == 1) {
            return GetTowingWheelAxisTowingConnector();
        } else {
            return GetBaseVehicleTowingConnector();
        }
    }

    private VehicleConnector GetBaseVehicleTowingConnector() {
        var baseGO = root.transform.Find("Base Box Collider (New)");
        var baseSize = baseGO.GetComponent<BoxCollider>().size;
        
        var inFrontOfBoxCollider = new Vector3(0, 0, baseSize.z * 0.5f);
        var worldAnchorRestPoint = root.transform.TransformPoint(inFrontOfBoxCollider);
        
        return new VehicleConnector {
            rigidbody = root.GetComponent<Rigidbody>(),
            anchorOffset = inFrontOfBoxCollider,
            worldAnchorRestPoint = worldAnchorRestPoint,
        };
    }

    private VehicleConnector GetTowingWheelAxisTowingConnector() {
        var towingWheelAxis = wheelAxes.Single(axis => axis.turningBody != null);
        var turningBodyBoxCollider = towingWheelAxis.turningBody.GetComponent<BoxCollider>();
        
        var inFrontOfTurningBodyCollider = new Vector3(0, 0, turningBodyBoxCollider.size.z);
        var anyWheelTransform = towingWheelAxis.leftWheel.transform;
        var wheelAxisCenter = new Vector3(0, anyWheelTransform.position.y, anyWheelTransform.position.z);
        var worldAnchorRestPoint = root.transform.TransformPoint(wheelAxisCenter + inFrontOfTurningBodyCollider);

        return new VehicleConnector {
            rigidbody = towingWheelAxis.turningBody,
            anchorOffset = inFrontOfTurningBodyCollider,
            worldAnchorRestPoint = worldAnchorRestPoint,
        };
    }

    public VehicleConnector GetPullingConnector() {
        var baseGO = root.transform.Find("Base Box Collider (New)");
        var baseSize = baseGO.GetComponent<BoxCollider>().size;
        
        var inBackOfBoxCollider = new Vector3(0, 0, -baseSize.z * 0.5f);
        var worldAnchorRestPoint = root.transform.TransformPoint(inBackOfBoxCollider);
        
        return new VehicleConnector {
            rigidbody = root.GetComponent<Rigidbody>(),
            anchorOffset = inBackOfBoxCollider,
            worldAnchorRestPoint = worldAnchorRestPoint,
        };
    }

    internal void BreakWheelsFrictionWithConstantTorque() {
        var flipFlop = 1;
        foreach (var axis in wheelAxes) {
            flipFlop = flipFlop < 0 ? 1 : -1;
            axis.leftWheel.motorTorque = flipFlop * 0.1f;
            axis.rightWheel.motorTorque = flipFlop * 0.1f;
        }
    }

    public void SetAxisMotorTorque(int axisIndex, float torque) {
        var axis = wheelAxes[axisIndex];
        axis.leftWheel.motorTorque = torque;
        axis.rightWheel.motorTorque = torque;
    }

    public void SetAxisBreaksTorque(int axisIndex, float torque) {
        var axis = wheelAxes[axisIndex];
        axis.leftWheel.brakeTorque = torque;
        axis.rightWheel.brakeTorque = torque;
    }

    public void SetAxisSteerAngle(int axisIndex, float steerAngleInDegrees) {
        var axis = wheelAxes[axisIndex];
        axis.leftWheel.steerAngle = steerAngleInDegrees;
        axis.rightWheel.steerAngle = steerAngleInDegrees;
    }

    public void GetAxisPose(int axisIndex, out Vector3 positionL, out Quaternion rotationL, out Vector3 positionR, out Quaternion rotationR) {
        var axis = wheelAxes[axisIndex];
        axis.leftWheel.GetWorldPose(out positionL, out rotationL);
        axis.rightWheel.GetWorldPose(out positionR, out rotationR);
    }

    public void GetTowingAxisPose(out Vector3 positionL, out Quaternion rotationL, out Vector3 positionR, out Quaternion rotationR, out Quaternion tipRotation) {
        var towingAxis = wheelAxes.Single(axis => axis.turningBody != null);
        towingAxis.leftWheel.GetWorldPose(out positionL, out rotationL);
        towingAxis.rightWheel.GetWorldPose(out positionR, out rotationR);
        tipRotation = towingAxis.turningBody.transform.rotation;
    }

    public bool IsDriveAxis(int axisIndex) {
        return wheelAxes[axisIndex].drive;
    }

    public bool IsSteerAxis(int axisIndex) {
        return wheelAxes[axisIndex].steer;
    }

    private Rigidbody CreateDefaultTurningBody(float upOffset, float forwardOffset, float length) {
        var turningBody = new GameObject("Turning Body (New)", typeof(Rigidbody), typeof(BoxCollider));
        turningBody.transform.SetParent(root.transform, worldPositionStays: false);
        turningBody.transform.localPosition = new Vector3(0, upOffset, forwardOffset);
        var collider = turningBody.GetComponent<BoxCollider>();
        collider.hideFlags = DefaultHideFlag;
        collider.center = new Vector3(0, 0, length * 0.5f);
        collider.size = new Vector3(0.025f, 0.025f, length);
        var rigidbody = turningBody.GetComponent<Rigidbody>();
        rigidbody.hideFlags = DefaultHideFlag;
        rigidbody.mass = 1;
        return rigidbody;
    }

    private WheelCollider CreateDefaultWheel(float radius) {
        var wheel = new GameObject("Default Wheel (New)", typeof(WheelCollider), typeof(WheelDebug));
        wheel.transform.hideFlags = DefaultHideFlag;
        wheel.transform.SetParent(root.transform, worldPositionStays: false);
        var wheelCollider = wheel.GetComponent<WheelCollider>();
        wheelCollider.hideFlags = DefaultHideFlag;
        wheelCollider.suspensionSpring = CreateDefaultJointSpring();
        wheelCollider.suspensionDistance = 0.1f;
        wheelCollider.radius = radius;
        wheelCollider.mass = 5;
        return wheelCollider;
    }

    private JointSpring CreateDefaultJointSpring() {
        return new JointSpring {
            targetPosition = .5f,
            spring = 3500,
            damper = 450,
        };
    }

}
using System;
using System.Collections.Generic;

using UnityEditor.Experimental.GraphView;

using UnityEngine;
using UnityEngine.Assertions;

public class VehiclePhysics {

    struct WheelAxis {
        public WheelCollider leftWheel;    
        public WheelCollider rightWheel;    
        public bool drive;
        public bool steer;
    }
    
    private readonly GameObject root;
    private readonly List<WheelAxis> wheelAxes = new ();

    public int AxisCount => wheelAxes.Count;
    public Vector3 Position => root.transform.position;
    public Quaternion Rotation => root.transform.rotation;

    public VehiclePhysics(Vector3 position, Transform container) {
        root = new GameObject("Vehicle Physics (New)", typeof(Rigidbody));
        root.transform.SetParent(container, worldPositionStays: false);
        root.transform.position = position;
        var rigidbody = root.GetComponent<Rigidbody>();
        rigidbody.position = position;
        rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        rigidbody.automaticCenterOfMass = false;
        rigidbody.centerOfMass = Vector3.zero;
        rigidbody.hideFlags = HideFlags.NotEditable;
        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
        rigidbody.mass = 150;
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

    internal void ConnectWithHinge(VehiclePhysics headPhysics, float headVehicleAnchorOffset, float thisAnchorOffset, float distanceBetween) {
        Assert.IsNull(root.GetComponent<ConfigurableJoint>());

        var connector = CreateDefaultConnector(distanceBetween);
        var connectorJoint = connector.GetComponent<ConfigurableJoint>();
        connectorJoint.anchor = new Vector3(0, 0, distanceBetween * 0.5f);
        connectorJoint.connectedBody = headPhysics.root.GetComponent<Rigidbody>();
        connectorJoint.connectedAnchor = new Vector3(0, 0, headVehicleAnchorOffset);

        var headConnectPoint = headPhysics.root.transform.TransformPoint(new Vector3(0, 0, headVehicleAnchorOffset));
        var connectorRigidbody = connector.GetComponent<Rigidbody>();
        connectorRigidbody.position = headConnectPoint - 0.5f * distanceBetween * headPhysics.root.transform.forward;
        connector.transform.position = connectorRigidbody.position;

        var tailJoint = root.AddComponent<ConfigurableJoint>();
        tailJoint.hideFlags = HideFlags.NotEditable;
        tailJoint.autoConfigureConnectedAnchor = false;
        tailJoint.xMotion = ConfigurableJointMotion.Locked;
        tailJoint.yMotion = ConfigurableJointMotion.Locked;
        tailJoint.zMotion = ConfigurableJointMotion.Locked;
        tailJoint.angularXMotion = ConfigurableJointMotion.Limited;
        tailJoint.highAngularXLimit = new SoftJointLimit { limit = 20 };
        tailJoint.lowAngularXLimit = new SoftJointLimit { limit = -20 };
        tailJoint.angularYMotion = ConfigurableJointMotion.Locked;
        tailJoint.angularZMotion = ConfigurableJointMotion.Locked;
        tailJoint.anchor = new Vector3(0, 0, thisAnchorOffset);
        tailJoint.connectedBody = connector.GetComponent<Rigidbody>();
        tailJoint.connectedAnchor = new Vector3(0, 0, -distanceBetween * 0.5f);
        tailJoint.connectedMassScale = 1;

        var headJoint = headPhysics.root.GetComponent<ConfigurableJoint>();
        if (headJoint != null) {
            headJoint.connectedMassScale = 0.5f;
        }

        BreakWheelsFrictionWithConstantTorque();
    }

    private void BreakWheelsFrictionWithConstantTorque() {
        foreach (var axis in wheelAxes) {
            axis.leftWheel.motorTorque = 0.1f;
            axis.rightWheel.motorTorque = 0.1f;
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

    public bool IsDriveAxis(int axisIndex) {
        return wheelAxes[axisIndex].drive;
    }

    public bool IsSteerAxis(int axisIndex) {
        return wheelAxes[axisIndex].steer;
    }

    private GameObject CreateDefaultConnector(float length) {
        var connector = new GameObject("Connector (New)", typeof(Rigidbody), typeof(BoxCollider), typeof(ConfigurableJoint));
        connector.transform.SetParent(root.transform.parent, worldPositionStays: false);
        var collider = connector.GetComponent<BoxCollider>();
        collider.hideFlags = HideFlags.NotEditable;
        collider.size = new Vector3(0.025f, 0.025f, length);
        var rigidbody = connector.GetComponent<Rigidbody>();
        rigidbody.hideFlags = HideFlags.NotEditable;
        rigidbody.mass = 10;
        rigidbody.useGravity = false;
        var joint = connector.GetComponent<ConfigurableJoint>();
        joint.hideFlags = HideFlags.NotEditable;
        joint.autoConfigureConnectedAnchor = false;
        joint.xMotion = ConfigurableJointMotion.Locked;
        joint.yMotion = ConfigurableJointMotion.Locked;
        joint.zMotion = ConfigurableJointMotion.Locked;
        joint.angularXMotion = ConfigurableJointMotion.Limited;
        joint.lowAngularXLimit = new SoftJointLimit { limit = -20, };
        joint.highAngularXLimit = new SoftJointLimit { limit = 20, };
        joint.angularYMotion = ConfigurableJointMotion.Free;
        joint.angularYLimit = new SoftJointLimit { limit = 40, };
        joint.angularZMotion = ConfigurableJointMotion.Locked;
        return connector;
    }

    private WheelCollider CreateDefaultWheel(float radius) {
        var wheel = new GameObject("Default Wheel (New)", typeof(WheelCollider));
        wheel.transform.hideFlags = HideFlags.NotEditable;
        wheel.transform.SetParent(root.transform, worldPositionStays: false);
        var wheelCollider = wheel.GetComponent<WheelCollider>();
        wheelCollider.hideFlags = HideFlags.NotEditable;
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
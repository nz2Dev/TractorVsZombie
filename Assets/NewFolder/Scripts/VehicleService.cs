using System;
using System.Collections.Generic;

using UnityEngine;

public class VehicleService {
    
    private List<VehiclePhysics> physicsRegistry = new ();
    private Transform physicsContainer;

    public VehicleService(Transform physicsContainer) {
        this.physicsContainer = physicsContainer;
    }

    public int CreateVehicle(Vector3 baseSize, WheelAxisData[] wheels, Vector3 position = default) {
        var vehiclePhysics = new VehiclePhysics(position, physicsContainer);
        vehiclePhysics.ConfigureBase(baseSize);
        
        foreach (var wheelAxis in wheels)
            vehiclePhysics.CreateWheelAxis(
                wheelAxis.halfLength * 2, 
                wheelAxis.upOffset, 
                wheelAxis.forwardOffset, 
                wheelAxis.radius, 
                wheelAxis.drive, 
                wheelAxis.stear
            );

        physicsRegistry.Add(vehiclePhysics);
        var lastVehicleIndex = physicsRegistry.Count - 1;
        return lastVehicleIndex;
    }

    public void SetVehicleSteer(int vehicleIndex, float steerDegrees) {
        var vehiclePhysics = physicsRegistry[vehicleIndex];
        for (int axisIndex = 0; axisIndex < vehiclePhysics.AxisCount; axisIndex++) {
            var steerAngle = vehiclePhysics.IsSteerAxis(axisIndex) ? steerDegrees : 0;
            vehiclePhysics.SetAxisSteerAngle(axisIndex, steerAngle);
        }
    }

    public void SetVehicleGasThrottle(int vehicleIndex, float v) {
        const float maxTorque = 400;
        const float minTorqueToEaseFriction = 0.1f;
        
        var engineTorque = v * maxTorque;
        var construction = physicsRegistry[vehicleIndex];
        for (int axisIndex = 0; axisIndex < construction.AxisCount; axisIndex++) {
            var torque = construction.IsDriveAxis(axisIndex) ? engineTorque : minTorqueToEaseFriction;
            construction.SetAxisMotorTorque(axisIndex, torque);
        }
    }

    public void SetVehicleBreaks(int vehicleIndex, float v) {
        const float maxBreaksTorque = 400;
        var construction = physicsRegistry[vehicleIndex];
        for (int axisIndex = 0; axisIndex < construction.AxisCount; axisIndex++) {
            construction.SetAxisBreaksTorque(axisIndex, v * maxBreaksTorque);
        }
    }

    public void MakeTowingConnection(int headVehicleIndex, int tailVehicleIndex, float anchorsOffset) {
        var headPhysics = physicsRegistry[headVehicleIndex];
        var tailPhysics = physicsRegistry[tailVehicleIndex];
        
        var towingConnector = tailPhysics.GetTowingConnector();
        var pullingConnector = headPhysics.GetPullingConnector();

        var pullJoint = towingConnector.rigidbody.gameObject.AddComponent<ConfigurableJoint>();
        pullJoint.hideFlags = HideFlags.NotEditable;
        pullJoint.xMotion = ConfigurableJointMotion.Locked;
        pullJoint.yMotion = ConfigurableJointMotion.Locked;
        pullJoint.zMotion = ConfigurableJointMotion.Locked;
        pullJoint.angularXMotion = ConfigurableJointMotion.Limited;
        pullJoint.highAngularXLimit = new SoftJointLimit { limit = 20 };
        pullJoint.lowAngularXLimit = new SoftJointLimit { limit = -20 };
        pullJoint.angularYMotion = ConfigurableJointMotion.Free;
        pullJoint.angularZMotion = ConfigurableJointMotion.Locked;
        pullJoint.connectedBody = pullingConnector.rigidbody;
        pullJoint.autoConfigureConnectedAnchor = false;
        var pullingOffset = anchorsOffset * 0.5f * Vector3.back;
        pullJoint.connectedAnchor = pullingConnector.anchorOffset + pullingOffset;
        var towingOffset = anchorsOffset * 0.5f * Vector3.forward;
        pullJoint.anchor = towingConnector.anchorOffset + towingOffset;

        tailPhysics.BreakWheelsFrictionWithConstantTorque();
    }

    public VehiclePose GetVehiclePose(int vehicleIndex) {
        var vehiclePhysics = physicsRegistry[vehicleIndex];
        return new VehiclePose {
            position = vehiclePhysics.Position,
            rotation = vehiclePhysics.Rotation
        };
    }

    public WheelAxisPose GetVehicleWheelAxisPose(int vehicleIndex, int axisIndex) {
        var vehiclePhysics = physicsRegistry[vehicleIndex];
        vehiclePhysics.GetAxisPose(axisIndex, out var positionL, out var rotationL, out var positionR, out var rotationR);
        return new WheelAxisPose {
            positionL = positionL,
            rotationL = rotationL,
            positionR = positionR,
            rotationR = rotationR
        };
    }

}
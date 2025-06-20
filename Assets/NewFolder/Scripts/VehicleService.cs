using System;
using System.Collections.Generic;

using UnityEngine;

public class VehicleService {
    
    private List<VehiclePhysics> physicsRegistry = new ();
    private Transform physicsContainer;

    public VehicleService(Transform physicsContainer) {
        this.physicsContainer = physicsContainer;
    }

    public int CreateVehicle(Vector3 baseSize, WheelAxisData[] wheels, TowingWheelAxisData? towingWheel = null, Vector3 position = default, float mass = 100) {
        var vehiclePhysics = new VehiclePhysics(position, physicsContainer, mass);
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
        
        if (towingWheel.HasValue) {
            vehiclePhysics.CreateTowingWheelAxis(
                towingWheel.Value.halfLength * 2,
                towingWheel.Value.upOffset,
                towingWheel.Value.forwardOffset,
                towingWheel.Value.radius,
                towingWheel.Value.towingBodyLength
            );
        }

        physicsRegistry.Add(vehiclePhysics);
        var lastVehicleIndex = physicsRegistry.Count - 1;
        return lastVehicleIndex;
    }

    public void UpdateVehicles() {
        foreach (var vehiclePhysics in physicsRegistry) {
            vehiclePhysics.UpdateTowingWheelAxis();
            
            var towingConnector = vehiclePhysics.GetTowingConnector();
            if (towingConnector.rigidbody.TryGetComponent<ConfigurableJoint>(out var towingJoint)) {
                var towingTip = towingJoint.transform.TransformPoint(towingJoint.anchor);
                var pullingTip = towingJoint.connectedBody.transform.TransformPoint(towingJoint.connectedAnchor);
                if (Vector3.Distance(towingTip, pullingTip) < 0.1f) {
                    towingJoint.zMotion = ConfigurableJointMotion.Locked;
                }
            }
        }
    }

    public void SetVehicleSteer(int vehicleIndex, float steerDegrees) {
        var vehiclePhysics = physicsRegistry[vehicleIndex];
        for (int axisIndex = 0; axisIndex < vehiclePhysics.AxisCount; axisIndex++) {
            var steerAngle = vehiclePhysics.IsSteerAxis(axisIndex) ? steerDegrees : 0;
            vehiclePhysics.SetAxisSteerAngle(axisIndex, steerAngle);
        }
    }

    public void SetVehicleGasThrottle(int vehicleIndex, float v) {
        const float maxTorque = 1000;
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

    public void MakeTowingConnection(int headVehicleIndex, int tailVehicleIndex, float anchorsOffset = 0) {
        var headPhysics = physicsRegistry[headVehicleIndex];
        var tailPhysics = physicsRegistry[tailVehicleIndex];
        
        var towingConnector = tailPhysics.GetTowingConnector();
        var pullingConnector = headPhysics.GetPullingConnector();

        var pullJoint = towingConnector.rigidbody.gameObject.AddComponent<ConfigurableJoint>();
        pullJoint.xMotion = ConfigurableJointMotion.Locked;
        pullJoint.yMotion = ConfigurableJointMotion.Locked;
        pullJoint.zMotion = ConfigurableJointMotion.Free;
        pullJoint.angularXMotion = ConfigurableJointMotion.Limited;
        pullJoint.angularYMotion = ConfigurableJointMotion.Free;
        pullJoint.angularZMotion = ConfigurableJointMotion.Locked;
        pullJoint.highAngularXLimit = new SoftJointLimit { limit = 20 };
        pullJoint.lowAngularXLimit = new SoftJointLimit { limit = -20 };
        pullJoint.zDrive = new JointDrive { positionSpring = 50_000,  positionDamper = 15_000, maximumForce = float.MaxValue };
        pullJoint.autoConfigureConnectedAnchor = false;
        pullJoint.connectedBody = pullingConnector.rigidbody;
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

    public TowingWheelAxisPose GetVehicleTowingWheelAxisPose(int vehicleIndex) {
        var vehiclePhysics = physicsRegistry[vehicleIndex];
        vehiclePhysics.GetTowingAxisPose(out var positionL, out var rotationL, out var positionR, out var rotationR, out var tipRotation);
        return new TowingWheelAxisPose {
            positionL = positionL,
            rotationL = rotationL,
            positionR = positionR,
            rotationR = rotationR,
            tipRotation = tipRotation
        };
    }

}
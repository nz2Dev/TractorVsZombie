using System.Collections.Generic;

using UnityEngine;

public class VehicleSystem {
    
    private readonly PoseRegistry poseRegistry;
    private readonly VehicleService vehicleService;

    private readonly Dictionary<VehicleId, VehicleModel> registry = new();
    private int idCounter;

    public VehicleSystem(VehicleService vehicleService, PoseRegistry poseRegistry) {
        this.vehicleService = vehicleService;
        this.poseRegistry = poseRegistry;
    }

    public VehicleId Create(Vector3 position, VehiclePrototype prototype, PoseId poseId, Quaternion rotation = default) {
        var id = new VehicleId(++idCounter);
        var vehiclePhysicsId = vehicleService.CreateVehicle(position, prototype.prefab, rotation);
        var model = new VehicleModel(id, vehiclePhysicsId, poseId);
        registry.Add(id, model);
        return id;
    }

    public void Remove(VehicleId id) {
        registry.Remove(id, out var model);
        vehicleService.DeleteVehicle(model.VehiclePhysicsId);
    }

    public bool Exists(VehicleId id) {
        return registry.ContainsKey(id);
    }

    public VehicleState ReadState(VehicleId id) {
        var model = registry[id];
        return new VehicleState {
            pose = vehicleService.ReadVehiclePose(model.VehiclePhysicsId),
            motorRev = model.Gas
        };
    }

    public void SetGas(VehicleId id, float gas) {
        registry[id].Gas = gas;
    }

    public void SetBrakes(VehicleId id, float brakes) {
        registry[id].Brakes = brakes;
    }

    public void SetSteer(VehicleId id, float steer) {
        var model = registry[id];
        model.Steer = steer;
        model.SteerIsAngle = false;
    }

    public void SetSteerAngle(VehicleId id, float steerAngle) {
        var model = registry[id];
        model.Steer = steerAngle;
        model.SteerIsAngle = true;
    }

    public void SetPowertrain(VehicleId id, float gas, float brakes) {
        var model = registry[id];
        model.Gas = gas;
        model.Brakes = brakes;
        vehicleService.SetVehiclePowertrain(model.VehiclePhysicsId, gas, brakes);
    }

    public virtual void Connect(VehicleId headId, VehicleId tailId) {
        var head = registry[headId];
        var headPose = vehicleService.ReadVehiclePose(head.VehiclePhysicsId);
        var tail = registry[tailId];
        var tailPose = vehicleService.ReadVehiclePose(tail.VehiclePhysicsId);
        var towardHeadRotation = Quaternion.LookRotation((headPose.position - tailPose.position).normalized, Vector3.up);
        vehicleService.UpdateVehiclePose(tail.VehiclePhysicsId, tailPose.position, towardHeadRotation);
        vehicleService.MakeTowingConnection(head.VehiclePhysicsId, tail.VehiclePhysicsId);
        poseRegistry.Write(tail.HostPoseId, tailPose.position, tailPose.rotation);
    }

    public virtual void Disconnect(VehicleId id) {
        vehicleService.ClearTowingConnection(registry[id].VehiclePhysicsId);
    }

    public float GetLinearDamping(VehicleId id) {
        return vehicleService.GetLinearDamping(registry[id].VehiclePhysicsId);
    }

    public void SetLinearDamping(VehicleId id, float linearDamping) {
        vehicleService.SetLinearDamping(registry[id].VehiclePhysicsId, linearDamping);
    }

    public void Update() {
        foreach (var model in registry.Values) {
            var pose = vehicleService.ReadVehiclePose(model.VehiclePhysicsId);
            poseRegistry.Write(model.HostPoseId, pose.position, pose.rotation);

            vehicleService.SetVehicleInput(model.VehiclePhysicsId, model.Gas, model.Brakes, model.SteerIsAngle ? 0 : model.Steer);
            if (model.SteerIsAngle) {
                vehicleService.SetVehicleSteerAngle(model.VehiclePhysicsId, model.Steer);
            }
        }
    }

}

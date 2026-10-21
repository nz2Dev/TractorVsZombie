using System.Collections.Generic;

using UnityEngine;

public class VehicleSystem {
    private readonly VehicleService vehicleService;
    private readonly PoseRegistry poseRegistry;
    private readonly Dictionary<VehicleId, VehicleModel> registry = new();
    private int idCounter;

    public VehicleSystem(VehicleService vehicleService, PoseRegistry poseRegistry) {
        this.vehicleService = vehicleService;
        this.poseRegistry = poseRegistry;
    }

    public VehicleId Create(Vector3 position, VehiclePrototype prototype, PoseId poseId, Quaternion rotation = default) {
        var id = new VehicleId(++idCounter);
        var vehiclePhysicsId = vehicleService.CreateVehicle(position, prototype.prefab, rotation);
        var model = new VehicleModel(id, vehiclePhysicsId, poseId, vehicleService.GetVehicleState(vehiclePhysicsId));
        registry.Add(id, model);
        return id;
    }

    public void Remove(VehicleId id) {
        if (!registry.Remove(id, out var model))
            return;

        vehicleService.DeleteVehicle(model.VehiclePhysicsId);
    }

    public bool Exists(VehicleId id) => registry.ContainsKey(id);

    public VehicleState ReadState(VehicleId id) => registry[id].State;

    public float ReadGas(VehicleId id) => registry[id].Gas;

    public void SetGas(VehicleId id, float gas) => registry[id].Gas = gas;

    public void SetBrakes(VehicleId id, float brakes) => registry[id].Brakes = brakes;

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
        var tail = registry[tailId];
        var towardHeadRotation = Quaternion.LookRotation((head.State.position - tail.State.position).normalized, Vector3.up);
        vehicleService.UpdateVehiclePose(tail.VehiclePhysicsId, tail.State.position, towardHeadRotation);
        vehicleService.MakeTowingConnection(head.VehiclePhysicsId, tail.VehiclePhysicsId);
        tail.State = vehicleService.GetVehicleState(tail.VehiclePhysicsId);
        poseRegistry.Write(tail.HostPoseId, tail.State.position, tail.State.rotation);
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
            model.State = vehicleService.GetVehicleState(model.VehiclePhysicsId);
            poseRegistry.Write(model.HostPoseId, model.State.position, model.State.rotation);

            vehicleService.SetVehicleInput(model.VehiclePhysicsId, model.Gas, model.Brakes, model.SteerIsAngle ? 0 : model.Steer);
            if (model.SteerIsAngle) {
                vehicleService.SetVehicleSteerAngle(model.VehiclePhysicsId, model.Steer);
            }
        }
    }

}

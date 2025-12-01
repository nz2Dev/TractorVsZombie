using System;
using System.Collections.Generic;

using UnityEngine;

public class TowableVehicleController {
    
    private readonly TowableVehicleView view;
    private readonly VehicleService vehicleService;

    private int idCounter;
    private readonly Dictionary<TowableVehicleId, TowableVehicleModel> registry = new ();

    public TowableVehicleController(TowableVehicleView view, VehicleService vehicleService) {
        this.view = view;
        this.vehicleService = vehicleService;
    }

    public void Update() {
        UpdateVehicleOrientation();
    }

    public TowableVehicleId SpawnVehicle(Vector3 position, TowableVehicleConfig vehicleConfig) {
        var nextId = new TowableVehicleId(++idCounter);
        var model = new TowableVehicleModel(nextId, position, vehicleConfig);
        registry[nextId] = model;
        model.PhysicsId = vehicleService.CreateVehicle(model.Position, model.PhysicsPrefab);
        view.AddVehicle(model.Id, model.Position, model.VisualsPrefab);
        return model.Id;
    }

    public void DeleteVehicle(TowableVehicleId id) {
        var model = registry[id];
        vehicleService.DeleteVehicle(model.PhysicsId);
        view.RemoveVehicle(model.Id);
        registry.Remove(model.Id);
    }

    public Vector3 GetVehiclePosition(TowableVehicleId id) {
        return registry[id].Position;
    }
    
    public int ReadVehiclePhysicsId(TowableVehicleId id) {
        return registry[id].PhysicsId;
    }

    public void ConnectVehicle(TowableVehicleId id, int headPhysicsId) {
        var tailModel = registry[id];
        var headState = vehicleService.GetVehicleState(headPhysicsId);
        var towardHeadRotation = Quaternion.LookRotation((headState.position - tailModel.Position).normalized, Vector3.up);
        vehicleService.UpdateVehiclePose(tailModel.PhysicsId, tailModel.Position, towardHeadRotation);
        vehicleService.MakeTowingConnection(headPhysicsId, tailModel.PhysicsId);
    }

    public void DisconnectVehicle(TowableVehicleId id) {
        var vehicle = registry[id];
        vehicleService.ClearTowingConnection(vehicle.PhysicsId);
    }

    private void UpdateVehicleOrientation() {
        foreach (var model in registry.Values) {
            model.PhysicsPose = vehicleService.GetVehicleState(model.PhysicsId);
            model.Position = model.PhysicsPose.position;
            view.UpdateVehiclePose(model.Id, model.PhysicsPose);
        }
    }

}
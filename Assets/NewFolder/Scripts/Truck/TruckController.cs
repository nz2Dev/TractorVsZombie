
using System;

using UnityEngine;

public class TruckController {
    
    private readonly TruckView view;
    private readonly CombatSystem combatSystem;
    private readonly DriverSimulator driverSimulator;
    private readonly VehicleService vehicleService;
    private readonly RamEffect ramEffect;

    private TruckModel model;

    public TruckController(CombatSystem combatSystem, RamEffect ramEffect, TruckView view, VehicleService vehicleService, DriverSimulator driverSimulator) {
        this.combatSystem = combatSystem;
        this.ramEffect = ramEffect;
        this.view = view;
        this.vehicleService = vehicleService;
        this.driverSimulator = driverSimulator;
    }

    public int ReadVehiclePhysicsId() => model.VehiclePhysicsId;
    public Vector3 ReadVehiclePosition() => model.Position;

    public void Update() {
        ReadExternalState();
        WriteExternalInput();
        UpdateView();
    }

    public void Spawn(Vector3 position, TruckConfig config) {
        model = new TruckModel(config);
        model.CombatId = combatSystem.RegisterAgent(position, alie: true);
        model.RamId = ramEffect.StartNew(position, model.CombatId, model.Config.ramConfig);
        model.DriverId = driverSimulator.Create(model.Config.driverConfig);
        model.VehiclePhysicsId = vehicleService.CreateVehicle(position, model.Config.vehiclePhysicsPrefab);
        view.Show(position, model.Config.visualsPrefab, model.Config.engineLoopSFX);
    }

    public void SetDrivingInput(float steerAmount, float gasAmount, bool boost) {
        driverSimulator.SetInput(model.DriverId, gasAmount, steerAmount, boost);
    }

    private void ReadExternalState() {
        model.VehiclePhysicsState = vehicleService.GetVehicleState(model.VehiclePhysicsId);
        model.Position = model.VehiclePhysicsState.position;
        model.DriverOutput = driverSimulator.GetOutput(model.DriverId);
    }

    private void WriteExternalInput() {
        ramEffect.Forward(model.RamId, model.Position);
        combatSystem.UpdateAgentPosition(model.CombatId, model.Position);
        driverSimulator.SetVehicleInput(model.DriverId, model.VehiclePhysicsState.velocity);
        vehicleService.SetVehicleInput(model.VehiclePhysicsId, 
            model.DriverOutput.motorTroque,
            model.DriverOutput.brakesTorque,
            model.DriverOutput.steeringDegrees);
    }

    private void UpdateView() {
        view.UpdatePose(model.VehiclePhysicsState);
        view.UpdateSound(model.DriverOutput.gasThrottle);
    }
}
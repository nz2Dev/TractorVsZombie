
using System;

using UnityEngine;

public class TruckController {
    
    private readonly TruckView view;
    private readonly CombatSystem combatSystem;
    private readonly VehicleService vehicleService;
    private readonly RamEffectController ramEffect;

    private TruckModel model;

    public TruckController(CombatSystem combatSystem, RamEffectController ramEffect, TruckView view, VehicleService vehicleService) {
        this.combatSystem = combatSystem;
        this.ramEffect = ramEffect;
        this.view = view;
        this.vehicleService = vehicleService;
    }

    public int ReadVehiclePhysicsId() => model.VehiclePhysicsId;
    public Vector3 ReadVehiclePosition() => model.Position;

    public void Update() {
        if (model == null)
            return;
            
        ReadExternalState();
        WriteExternalInput();
        UpdateView();
    }

    public void Create(TruckPrototype prototype, Vector3 position = default) {
        model = new TruckModel(prototype.config, position == default ? prototype.position : position);
        model.CombatId = combatSystem.RegisterAgent(model.Position, alie: true);
        model.RamId = ramEffect.StartNew(model.CombatId, prototype.ramPrototype);
        model.VehiclePhysicsId = vehicleService.CreateVehicle(model.Position, prototype.vehiclePhysicsPrefab, prototype.rotation);
        view.Show(model.Position, prototype.visualsPrefab, prototype.engineLoopSFX);
    }

    public void Drive(float driveInput, bool boostInput) {
        model.MotorTorque = VehicleDriving.GasThrottle(driveInput, boostInput, model.Config.drivingConfig.maxEngineTorque);   
    }

    public void Steer(float steerInput) {
        var steeringLimit = VehicleDriving.LimitSteering(model.VehiclePhysicsState.velocity.magnitude, model.Config.drivingConfig);
        model.SteeringDegrees = steerInput * steeringLimit * model.Config.drivingConfig.maxSteerDegrees;
    }

    private void ReadExternalState() {
        model.VehiclePhysicsState = vehicleService.GetVehicleState(model.VehiclePhysicsId);
        model.Position = model.VehiclePhysicsState.position;
    }

    private void WriteExternalInput() {
        ramEffect.Forward(model.RamId, model.Position);
        combatSystem.UpdateAgentPosition(model.CombatId, model.Position);
        vehicleService.SetVehicleInput(model.VehiclePhysicsId, model.MotorTorque, brakesTorque: 0, model.SteeringDegrees);
    }

    private void UpdateView() {
        view.UpdatePose(model.VehiclePhysicsState);
        var motorRev = model.MotorTorque / Mathf.Max(model.Config.drivingConfig.maxEngineTorque, 1);
        view.UpdateSound(motorRev);
    }

}
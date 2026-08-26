using Combat;

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

    public virtual int ReadVehiclePhysicsId() => model.VehiclePhysicsId;
    public virtual Vector3 ReadVehiclePosition() => model.Position;

    public void Update() {
        if (model == null)
            return;

        ReadExternalState();
        WriteExternalInput();
        UpdateView();
    }

    public virtual void Create(TruckPrototype prototype, Vector3 position = default) {
        model = new TruckModel(prototype.config, position == default ? prototype.position : position);
        model.CombatId = combatSystem.Add(prototype.combatPrototype);
        model.RamId = ramEffect.StartNew(model.CombatId, prototype.combatPrototype.alie, prototype.ramPrototype);
        model.VehiclePhysicsId = vehicleService.CreateVehicle(model.Position, prototype.vehiclePrefab, prototype.rotation);
        view.Show(model.Position, prototype.visualsPrefab, prototype.engineLoopSFX);
    }

    public virtual void Drive(float driveInput, bool boostInput) {
        var boostMultiplier = boostInput ? 2f : 1f;   // gameplay rule: boost doubles throttle
        model.Gas = driveInput * boostMultiplier;
    }

    public virtual void Brake(float brakes) {
        model.Brakes = brakes;
    } 

    public virtual void Steer(float steerInput) {
        model.Steer = steerInput;   // -1..1; traction limiting happens inside VehiclePhysics
    }

    private void ReadExternalState() {
        model.VehiclePhysicsState = vehicleService.GetVehicleState(model.VehiclePhysicsId);
        model.Position = model.VehiclePhysicsState.position;
    }

    private void WriteExternalInput() {
        ramEffect.Forward(model.RamId, model.Position);
        // todo: register proxmity and raycast components
        vehicleService.SetVehicleInput(model.VehiclePhysicsId, model.Gas, model.Brakes, model.Steer);
    }

    private void UpdateView() {
        view.UpdatePose(model.VehiclePhysicsState);
        view.UpdateSound(model.Gas);
    }

}

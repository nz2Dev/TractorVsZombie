
using UnityEngine;

public class TruckController {
    
    private readonly CombatSystem combatSystem;
    private readonly MotorVehicleController motorVehicleController;
    private readonly RamEffect ramEffect;

    private TruckModel model;

    public TruckController(CombatSystem combatSystem, MotorVehicleController vehicleController, RamEffect ramEffect) {
        this.combatSystem = combatSystem;
        this.motorVehicleController = vehicleController;
        this.ramEffect = ramEffect;
    }

    public MotorVehicleId ReadVehicleId() => model.VehicleId;
    public Vector3 ReadVehiclePosition() => model.Position;

    public void Update() {
        SyncPosition();
    }

    public void Spawn(Vector3 position, TruckConfig config) {
        model = new TruckModel(config);
        model.CombatId = combatSystem.RegisterAgent(position, alie: true);
        model.VehicleId = motorVehicleController.SpawnVehicle(position, model.VehicleConfig);
        model.RamId = ramEffect.StartNew(position, model.CombatId, model.RamConfig);
    }

    public void Control(float steerAmount, float gasAmount, bool boost) {
        motorVehicleController.SteerVehicle(model.VehicleId, steerAmount);
        motorVehicleController.DriveVehicle(model.VehicleId, gasAmount, boost);
    }

    private void SyncPosition() {
        model.Position = motorVehicleController.GetVehiclePosition(model.VehicleId);
        combatSystem.UpdateAgentPosition(model.CombatId, model.Position);
        ramEffect.Forward(model.RamId, model.Position);
    }
}
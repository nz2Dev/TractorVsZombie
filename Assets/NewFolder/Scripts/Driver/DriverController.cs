
using UnityEngine;

public class DriverController {
    
    private readonly CombatService combatService;
    private readonly MotorVehicleController motorVehicleController;

    private DriverModel model;

    public DriverController(CombatService combatService, MotorVehicleController vehicleController) {
        this.combatService = combatService;
        this.motorVehicleController = vehicleController;
    }

    public MotorVehicleId ReadVehicleId() => model.VehicleId;
    public Vector3 ReadVehiclePosition() => model.Position;

    public void Update() {
        SyncPosition();
    }

    public void Spawn(Vector3 position, DriverConfig config) {
        model = new DriverModel(config);
        model.CombatId = combatService.RegisterAgent(position, alie: true);
        model.VehicleId = motorVehicleController.SpawnVehicle(position, model.CombatId, model.VehicleConfig);
    }

    public void Control(float steerAmount, float gasAmount, bool boost) {
        motorVehicleController.SteerVehicle(model.VehicleId, steerAmount);
        motorVehicleController.DriveVehicle(model.VehicleId, gasAmount, boost);
    }

    private void SyncPosition() {
        model.Position = motorVehicleController.GetVehiclePosition(model.VehicleId);
        combatService.UpdateAgentPosition(model.CombatId, model.Position);
    }
}

using UnityEngine;

public class DriverController {
    
    private readonly CombatService combatService;
    private readonly VehicleController vehicleController;

    private DriverModel model;

    public DriverController(CombatService combatService, VehicleController vehicleController) {
        this.combatService = combatService;
        this.vehicleController = vehicleController;
    }

    public int ReadVehicleId() => model.VehicleId;
    public Vector3 ReadVehiclePosition() => model.Position;

    public void Update() {
        SyncPosition();
    }

    public void Spawn(Vector3 position, DriverConfig config) {
        model = new DriverModel(config);
        model.CombatId = combatService.RegisterAgent(position, alie: true);
        model.VehicleId = vehicleController.SpawnVehicle(position, model.CombatId, model.VehicleConfig);
    }

    public void Control(float steerAmount, float gasAmount, bool boost) {
        vehicleController.SteerVehicle(model.VehicleId, steerAmount);
        vehicleController.DriveVehicle(model.VehicleId, gasAmount, boost);
    }

    private void SyncPosition() {
        model.Position = vehicleController.GetVehiclePosition(model.VehicleId);
        combatService.UpdateAgentPosition(model.CombatId, model.Position);
    }
}
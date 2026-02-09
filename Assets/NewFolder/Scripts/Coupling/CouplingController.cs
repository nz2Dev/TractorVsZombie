public class CouplingController {
    
    private readonly MotorVehicleController motorVehicleController;
    private readonly TowableVehicleController towableVehicleController;

    private CouplingModel model;

    public CouplingController(MotorVehicleController motorVehicleController, TowableVehicleController towableVehicleController) {
        this.motorVehicleController = motorVehicleController;
        this.towableVehicleController = towableVehicleController;
    }

    public void Create(int pullingVehiclePhysicsId) {
        model = new CouplingModel();
        model.PullingVehiclePhysicsId = pullingVehiclePhysicsId;
    }

    public void AddTowable(TowableVehicleId vehicleId) {
        int lastVehiclePhysicsId;
        if (model.TowableVehicleIds.Count != 0) {
            var lastTowableVehicleId = model.TowableVehicleIds[^1];
            lastVehiclePhysicsId = towableVehicleController.ReadVehiclePhysicsId(lastTowableVehicleId);
        } else {
            lastVehiclePhysicsId = model.PullingVehiclePhysicsId;
        }

        towableVehicleController.ConnectVehicle(vehicleId, lastVehiclePhysicsId);
        model.TowableVehicleIds.Add(vehicleId);
    }

    public void InsertTowable(TowableVehicleId newVehicleId) {
        if (model.TowableVehicleIds.Count > 0) {
            var firstOrderTowable = model.TowableVehicleIds[0];
            towableVehicleController.DisconnectVehicle(firstOrderTowable);

            var newVehiclePhysicsId = towableVehicleController.ReadVehiclePhysicsId(newVehicleId);
            towableVehicleController.ConnectVehicle(firstOrderTowable, newVehiclePhysicsId);
        }

        towableVehicleController.ConnectVehicle(newVehicleId, model.PullingVehiclePhysicsId);
        model.TowableVehicleIds.Insert(0, newVehicleId);
    }

}
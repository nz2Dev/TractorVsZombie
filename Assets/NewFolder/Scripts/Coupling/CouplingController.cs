public class CouplingController {
    
    private readonly MotorVehicleController motorVehicleController;
    private readonly TowableVehicleController towableVehicleController;

    private CouplingModel model;

    public CouplingController(MotorVehicleController motorVehicleController, TowableVehicleController towableVehicleController) {
        this.motorVehicleController = motorVehicleController;
        this.towableVehicleController = towableVehicleController;
    }

    public void Create(MotorVehicleId pullingVehicleId) {
        model = new CouplingModel();
        model.PullingVehicleId = pullingVehicleId;
    }

    public void AddTowable(TowableVehicleId vehicleId) {
        int lastVehiclePhysicsId;
        if (model.TowableVehicleIds.Count != 0) {
            var lastTowableVehicleId = model.TowableVehicleIds[^1];
            lastVehiclePhysicsId = towableVehicleController.ReadVehiclePhysicsId(lastTowableVehicleId);
        } else {
            var pullingVehicleId = model.PullingVehicleId;
            lastVehiclePhysicsId = motorVehicleController.ReadVehiclePhysicsId(pullingVehicleId);
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

        var pullingVehiclePhysicsId = motorVehicleController.ReadVehiclePhysicsId(model.PullingVehicleId);
        towableVehicleController.ConnectVehicle(newVehicleId, pullingVehiclePhysicsId);
        model.TowableVehicleIds.Insert(0, newVehicleId);
    }

}
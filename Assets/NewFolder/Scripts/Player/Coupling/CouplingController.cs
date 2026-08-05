public class CouplingController {

    private readonly PlatformController platformController;
    private readonly TruckController truckController;
    
    private readonly CouplingModel model;

    public CouplingController(PlatformController platformController, TruckController truckController) {
        this.platformController = platformController;
        this.truckController = truckController;
        this.model = new CouplingModel();
    }

    public void CouplePlatformToTheEnd(int platformId) {
        int targetVehiclePhysicsId;
        if (model.CoupledPlatformIds.Count > 0) {
            var lastPlatformId = model.CoupledPlatformIds[^1];
            targetVehiclePhysicsId = platformController.GetVehiclePhysicsId(lastPlatformId);
        } else {
            targetVehiclePhysicsId = truckController.ReadVehiclePhysicsId();
        }

        platformController.Connect(platformId, targetVehiclePhysicsId);
        model.CoupledPlatformIds.Add(platformId);
    }

    public void CouplePlatformInFront(int platformId) {
        if (model.CoupledPlatformIds.Count > 0) {
            var firstPlatformId = model.CoupledPlatformIds[0];
            platformController.Disconnect(firstPlatformId);

            var newPlatformVehiclePhysicsId = platformController.GetVehiclePhysicsId(platformId);
            platformController.Connect(firstPlatformId, newPlatformVehiclePhysicsId);
        }

        platformController.Connect(platformId, truckController.ReadVehiclePhysicsId());
        model.CoupledPlatformIds.Insert(0, platformId);
    }
}
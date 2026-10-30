using UnityEngine;
public class PlayerController {

    private readonly DrivingController drivingController;
    private readonly AssemblingController assemblingController;
    private readonly LoaderController loaderController;
    private readonly SelectingController selectingController;
    private readonly AimingController aimingController;
    private readonly CollectingController collectingController;
    private readonly CameraController cameraController;

    public PlayerController(DrivingController drivingController, AssemblingController assemblingController, LoaderController loaderController,
        SelectingController selectingController, AimingController aimingController, 
        CollectingController collectingController, CameraController cameraController) {
        
        this.drivingController = drivingController;
        this.assemblingController = assemblingController;
        this.loaderController = loaderController;
        this.selectingController = selectingController;
        this.aimingController = aimingController;
        this.collectingController = collectingController;
        this.cameraController = cameraController;
    }

    public bool HasUnits => assemblingController.ControlledUnitsCount > 0;
    public bool HasPosition => assemblingController.HasPosition;
    public Vector3 Position => assemblingController.HeadPosition;

    public void Setup(PlayerPrototype prototype) {     
        aimingController.Init(prototype.aimingPrototype);
        collectingController.Init(prototype.collectingPrototype);
        assemblingController.Init(prototype.assemblingPrototype);
        
        assemblingController.SpawnDriver(prototype.initialTruckPrototype);
        Vector3 directionStep = prototype.initialTruckPrototype.rotation * Vector3.back * 6;
        Vector3 platformPosition = prototype.initialTruckPrototype.position + directionStep;
        for (int i = 0; i < prototype.initialPlatformCount; i++) {
            assemblingController.AddPlatform(platformPosition, trueInFront_falseToTheEnd: false);
            platformPosition += directionStep;
        }
        
        foreach (var loadout in prototype.initialLoadouts ?? new LoadoutPrototype[0]) {
            loaderController.Place(loadout);
        }       
    }

    public void Update() {
        if (!HasUnits)
            return;
        
        assemblingController.Update();
        drivingController.Update();
        selectingController.Update();
        aimingController.Update();
        collectingController.Update();
        cameraController.Update();
    }

}

using UnityEngine.UIElements;

public class PlayerController {

    private PlayerModel model;
    private readonly DrivingController drivingController;
    private readonly AssemblingController assemblingController;
    private readonly SelectingController selectingController;
    private readonly AimingController aimingController;
    private readonly CollectingController collectingController;
    private readonly CameraController cameraController;

    public PlayerController(DrivingController drivingController, AssemblingController assemblingController, 
        SelectingController selectingController, AimingController aimingController, 
        CollectingController collectingController, CameraController cameraController) {
        
        this.drivingController = drivingController;
        this.assemblingController = assemblingController;
        this.selectingController = selectingController;
        this.aimingController = aimingController;
        this.collectingController = collectingController;
        this.cameraController = cameraController;

        selectingController.OnSelectedPlatformChanged += () =>
            aimingController.SetManualPlatformIds(selectingController.SelectedPlatformIds);

        assemblingController.OnPlatformAdded += (platformId) => {
            selectingController.AddOption(platformId);
            aimingController.AddControlledPlatformId(platformId);
        };

        collectingController.OnLoadoutCollected += (position, loadoutPrototype) =>
            assemblingController.AddLoadout(position, loadoutPrototype, model.Config.startOrEndCouplingOfRewards);
    }

    public void Setup(PlayerPrototype prototype) {     
        model = new PlayerModel(prototype.config);
        aimingController.Init(prototype.aimingPrototype);
        assemblingController.Init(prototype.assemblingPrototype);
        collectingController.Init(prototype.collectingPrototype);
    }

    public void Update() {
        if (model == null)
            return;
        
        var headPosition = assemblingController.HeadPosition;
        drivingController.Update();
        selectingController.Update();
        assemblingController.Update();
        aimingController.SetAimSourcePosition(headPosition);
        aimingController.Update();
        collectingController.SetPosition(headPosition);
        collectingController.Update();
        cameraController.SetVehiclePosition(headPosition);
        cameraController.Update();
    }

}
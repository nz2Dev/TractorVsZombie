using System.Collections.Generic;

using UnityEngine;

public class GoalsController {
    
    private readonly PathfindingService pathfindingService;
    private readonly PlatformController platformController;
    private readonly TruckController truckController;

    private GoalsModel model;
    private readonly List<PlatformState> platformStatesBuffer = new();

    public int MainGoalFlowField => model.MainGoalFlowField;
    public int TargetFlowField => model.TargetFlowField;

    public GoalsController(PathfindingService pathfindingService, PlatformController platformController, TruckController truckController) {
        this.pathfindingService = pathfindingService;
        this.platformController = platformController;
        this.truckController = truckController;
    }

    public void Init(GoalsPrototype prototype) {
        model = new ();
        model.MainGoalFlowField = pathfindingService.CreateFlowField(prototype.mainRoute);
        model.TargetFlowField = pathfindingService.CreateFlowField(Vector3.zero);
        model.AlternativeGoal = prototype.alternativeRoute;
        model.MainGoal = prototype.mainRoute;
    }

    public void Update() {
        ReadGoalToggle();
        if (Time.frameCount % 2 == 0)
            TrackTargets();
    }

    private void ReadGoalToggle() {
        if (Input.GetKeyDown(KeyCode.R)) {
            var switchedStrategyToChaseCenter = !model.ChasingMainGoal;
            var targetPosition = switchedStrategyToChaseCenter ? model.MainGoal : model.AlternativeGoal;
            pathfindingService.UpdateGoal(model.MainGoalFlowField, targetPosition);
            model.ChasingMainGoal = switchedStrategyToChaseCenter;
        }
    }

    private void TrackTargets() {
        var count = 0;
        var center = Vector3.zero;
        
        if (truckController.UnitExist) {
            center += truckController.ReadVehiclePosition();
            count++;
        }
        
        platformController.ReadAllPlatforms(platformStatesBuffer);
        foreach (var platform in platformStatesBuffer) {
            center += platform.position;
            count++;
        }
        
        center = count > 0 ? center / count : center;
        pathfindingService.UpdateGoal(model.TargetFlowField, center);
    }
}
using System.Collections.Generic;

using UnityEngine;

public class GoalsController {
    
    private readonly PathfindingService pathfindingService;
    private readonly PlatformController platformController;

    private GoalsModel model;
    private readonly List<PlatformState> platformStatesBuffer = new();

    public int MainGoalFlowField => model.MainGoalFlowField;
    public int TargetFlowField => model.TargetFlowField;

    public GoalsController(PathfindingService pathfindingService, PlatformController platformController) {
        this.pathfindingService = pathfindingService;
        this.platformController = platformController;
    }

    public void Init(GoalsPrototype prototype) {
        model = new ();
        model.MainGoalFlowField = pathfindingService.CreateFlowField(Vector3.zero);
        model.TargetFlowField = pathfindingService.CreateFlowField(Vector3.zero);
        model.AlternativeGoal = prototype.alternativeRoute;
        model.MainGoal = prototype.mainRoute;
    }

    public void Update() {
        ReadGoalToggle();
        if (Time.frameCount % 2 == 0)
            TrackPlatforms();
    }

    private void ReadGoalToggle() {
        if (Input.GetKeyDown(KeyCode.R)) {
            var switchedStrategyToChaseCenter = !model.ChasingMainGoal;
            var targetPosition = switchedStrategyToChaseCenter ? model.MainGoal : model.AlternativeGoal;
            pathfindingService.UpdateGoal(model.MainGoalFlowField, targetPosition);
            model.ChasingMainGoal = switchedStrategyToChaseCenter;
        }
    }

    private void TrackPlatforms() {
        platformController.ReadAllPlatforms(platformStatesBuffer);
        var center = Vector3.zero;
        foreach (var platform in platformStatesBuffer) {
            center += platform.position;
        }
        center /= platformStatesBuffer.Count;
        pathfindingService.UpdateGoal(model.TargetFlowField, center);
    }
}
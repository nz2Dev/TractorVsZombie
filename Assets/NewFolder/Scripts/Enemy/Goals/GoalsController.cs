using UnityEngine;

public class GoalsController {
    
    private readonly PathfindingService pathfindingService;

    private GoalsModel model;

    public bool AnyGoalsChanged => model.ChangesRegistered;
    public int MainGoalFlowField => model.MainGoalFlowField;

    public GoalsController(PathfindingService pathfindingService) {
        this.pathfindingService = pathfindingService;
    }

    public void Init(GoalsPrototype prototype) {
        model = new ();
        model.MainGoalFlowField = pathfindingService.CreateFlowField(Vector3.zero);
        model.ChangesRegistered = true;
        model.MainGoal = prototype.mainRoute;
        model.AlternativeGoal = prototype.alternativeRoute;
    }

    public void Update() {
        ReadGoalToggle();
    }

    private void ReadGoalToggle() {
        model.ChangesRegistered = false;
        if (Input.GetKeyDown(KeyCode.R)) {
            var switchedStrategyToChaseCenter = !model.ChasingMainGoal;
            var targetPosition = switchedStrategyToChaseCenter ? model.MainGoal : model.AlternativeGoal;
            pathfindingService.UpdateGoal(model.MainGoalFlowField, targetPosition);
            model.ChasingMainGoal = switchedStrategyToChaseCenter;
            model.ChangesRegistered = true;
        }
    }
}
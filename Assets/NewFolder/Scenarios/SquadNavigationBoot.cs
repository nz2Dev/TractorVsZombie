using UnityEditor;

using UnityEngine;

// TODO: remove this
public class SquadNavigationBoot : MonoBehaviour {

    public static SquadNavigationBoot Instance;
    
    public int agentsLayer;
    public int foeAgentsLayer;
    public LayerMask obstaclesMask;

    private CombatSystem combatSystem;
    private RewardController rewardController;
    public InfantryController infantryController;
    public SquadAIController squadController;

    private void Awake() {
        var physicsService = new PhysicsService(null);
        var avoidanceService = new LocalAvoidanceService();
        var pathfindingService = new PathfindingService(FlowFieldSystem.Instance);
        
        combatSystem = new CombatSystem(null, null);
        
        rewardController = new RewardController(new RewardView());
        infantryController = new InfantryController(combatSystem, new InfantryView(), rewardController, physicsService, avoidanceService);
        squadController = new SquadAIController(infantryController, pathfindingService, combatSystem);
        
        Instance = this;
    }

    private void Update() {
        combatSystem.Update();
        rewardController.Update();
        infantryController.Update();
        squadController.Update();
    }

    private void OnDestroy() {
        rewardController.Destroy();
    }

}
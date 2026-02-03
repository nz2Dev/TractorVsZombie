using Codice.Client.Common.GameUI;

using Unity.Profiling;

using UnityEngine;
using UnityEngine.UIElements;

public class GameBootstrapper : MonoBehaviour {
    
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private FlowFieldSurface flowFieldSurface;
    [SerializeField] private ORCAEnvironment orcaEnvironment;
    [Space]
    [SerializeField] private string physicsServiceLayer;
    [SerializeField] private string combatServiceLayer;
    [SerializeField] private string foeCombatServiceLayer;
    [SerializeField] private string vehicleObstacleLayer;
    [SerializeField] private string physicsObstacleLayer;
    [SerializeField] private LayerMask combatServiceEnvironmentMask;
    [Space]
    [SerializeField] int unitsCount = 10;
    [SerializeField] private int maxVehicelCount = 10;
    [Space]
    [SerializeField] private PlayerConfig playerConfig;
    [SerializeField] private AimVisuals aimVisualsPrefab;
    [SerializeField] private UIDocument uiDocument;
    [Space]
    [SerializeField] private string rewardsLayerName; // FIXME: not used
    [SerializeField] private GameObject rewardVisualsPrefab;

    private CombatSystem combatSystem;

    private SpawningService spawningService;
    private BodySimulator bodySimulator;
    private NavigationSystem navigationSystem;
    private PlayerController playerController;
    private EnemyController enemyController;
    private RewardController rewardController;
    private ProjectileController projectileController;
    private RocketController rocketController;
    private WeaponController weaponController;
    private RamEffect ramEffect;
    private MotorVehicleController motorVehicleController;
    private TowableVehicleController towableVehicleController;
    private InfantryController infantryController;
    private ArmorController armorController;
    private PlatformController platformController;
    private DriverController driverController;
    private ArmorAIController armorAIController;
    private BehaviorSystem behaviorSystem;
    private CouplingController couplingController;
    private CommanderSystem commanderSystem;
    private ProductionBuildingController buildingController;
    private HeadquarterBuildingController headquarterBuildingController;

    private void Start() {
        Build();
        Init();
    }

    private void Build() {
        var vehicleService = new VehicleService(LayerMask.NameToLayer(vehicleObstacleLayer));
        var localAvoidanceService = new LocalAvoidanceService(orcaEnvironment);
        var pathfindingService = new PathfindingService(flowFieldSurface);
        var physicsService = new PhysicsService(
            container: null, 
            operationalLayer: LayerMask.NameToLayer(physicsServiceLayer), 
            obstacleLayer: LayerMask.NameToLayer(physicsObstacleLayer)
        );

        var playerView = new PlayerView(uiDocument, aimVisualsPrefab);
        var unitView = new EnemyView();
        var weaponView = new WeaponView();
        var motorVehicleView = new MotorVehicleView();
        var towableVehicleView = new TowableVehicleView();
        var rewardView = new RewardView(rewardVisualsPrefab);
        var infantryView = new InfantryView();
        var rocketView = new RocketView();
        var projectileView = new ProjectileView();
        var productionBuildingView = new ProductionBuildingView();
        
        combatSystem = new CombatSystem(
            LayerMask.NameToLayer(combatServiceLayer), 
            LayerMask.NameToLayer(foeCombatServiceLayer), 
            combatServiceEnvironmentMask
        );

        bodySimulator = new BodySimulator(
            physicsService
        );

        navigationSystem = new NavigationSystem(
            localAvoidanceService,
            pathfindingService
        );

        projectileController = new ProjectileController(
            combatSystem,
            soundManager,
            projectileView
        );

        rocketController = new RocketController(
            rocketView, 
            soundManager, 
            combatSystem
        );

        ramEffect = new RamEffect(
            combatSystem,
            soundManager
        );

        rewardController = new RewardController(
            rewardView
        );

        weaponController = new WeaponController(
            weaponView,
            rocketController,
            projectileController
        );

        motorVehicleController = new MotorVehicleController(
            motorVehicleView,
            vehicleService,
            soundManager
        );

        towableVehicleController = new TowableVehicleController(
            towableVehicleView,
            vehicleService
        );

        infantryController = new InfantryController(
            combatSystem,
            bodySimulator,
            infantryView,
            rewardController
        );

        armorController = new ArmorController(
            combatSystem,
            weaponController,
            motorVehicleController,
            ramEffect,
            rewardController
        );

        platformController = new PlatformController(
            combatSystem,
            towableVehicleController,
            weaponController,
            ramEffect
        );

        driverController = new DriverController(
            combatSystem,
            motorVehicleController,
            ramEffect
        );

        spawningService = new SpawningService(
            infantryController,
            armorController,
            maxVehicelCount,
            unitsCount
        );

        armorAIController = new ArmorAIController(
            combatSystem,
            pathfindingService,
            armorController,
            motorVehicleController,
            weaponController
        );

        behaviorSystem = new BehaviorSystem(
            navigationSystem,
            infantryController,
            combatSystem
        );

        commanderSystem = new CommanderSystem(
            behaviorSystem,
            navigationSystem
        );

        couplingController = new CouplingController(
            motorVehicleController,
            towableVehicleController
        );

        buildingController = new ProductionBuildingController(
            combatSystem,
            spawningService,
            behaviorSystem,
            commanderSystem,
            armorAIController,
            productionBuildingView
        );

        headquarterBuildingController = new HeadquarterBuildingController(
            combatSystem,
            pathfindingService,
            vehicleService,
            physicsService
        );

        playerController = new PlayerController(
            playerView, 
            new PlayerInput(),
            playerConfig,
            physicsService,
            combatSystem,
            cameraManager,
            rewardController,
            weaponController,
            platformController,
            driverController,
            couplingController,
            headquarterBuildingController
        );

        enemyController = new EnemyController(
            unitView,
            buildingController,
            commanderSystem
        );
    }

    private void Init() {
        playerController.Init();
    }

    private void Update() {
        combatSystem.Update();

        ramEffect.Update();
        bodySimulator.Update();
        rewardController.Update();
        navigationSystem.Update();

        projectileController.Update();
        rocketController.Update();
        weaponController.Update();
        motorVehicleController.Update();
        towableVehicleController.Update();
        infantryController.Update();
        armorController.Update();
        platformController.Update();
        driverController.Update();
        
        armorAIController.Update();
        behaviorSystem.Update();
        commanderSystem.Update();

        buildingController.Update();
        headquarterBuildingController.Update();

        enemyController.Update();
        playerController.Update();

    }

}

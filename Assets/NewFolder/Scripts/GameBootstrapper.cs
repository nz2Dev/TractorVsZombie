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
    [SerializeField] private LayerMask combatServiceEnvironmentMask;
    [Space]
    [SerializeField] int unitsCount = 10;
    [SerializeField] private int maxVehicelCount = 10;
    [SerializeField] Transform targetPoint;
    [Space]
    [SerializeField] private PlayerConfig playerConfig;
    [SerializeField] private AimVisuals aimVisualsPrefab;
    [SerializeField] private UIDocument uiDocument;
    [Space]
    [SerializeField] private string rewardsLayerName;
    [SerializeField] private GameObject rewardVisualsPrefab;

    private CombatService combatService;

    private SpawnSystem spawnSystem;
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

    private void Start() {
        Build();
        Init();
    }

    private void Build() {
        combatService = new CombatService(LayerMask.NameToLayer(combatServiceLayer), LayerMask.NameToLayer(foeCombatServiceLayer), combatServiceEnvironmentMask);
        var vehicleService = new VehicleService();
        var localAvoidanceService = new LocalAvoidanceService(orcaEnvironment);
        var pathfindingService = new PathfindingService(flowFieldSurface);
        var physicsService = new PhysicsService(container: null, LayerMask.NameToLayer(physicsServiceLayer));
        var rewardsMediator = new RewardsMediator(LayerMask.NameToLayer(rewardsLayerName));

        var playerView = new PlayerView(uiDocument, aimVisualsPrefab);
        var unitView = new EnemyView();
        var weaponView = new WeaponView();
        var motorVehicleView = new MotorVehicleView();
        var towableVehicleView = new TowableVehicleView();
        var rewardView = new RewardView(rewardVisualsPrefab);
        var infantryView = new InfantryView();
        var rocketView = new RocketView();
        var projectileView = new ProjectileView();

        bodySimulator = new BodySimulator(
            physicsService
        );

        navigationSystem = new NavigationSystem(
            localAvoidanceService,
            pathfindingService
        );

        projectileController = new ProjectileController(
            combatService,
            soundManager,
            projectileView
        );

        rocketController = new RocketController(
            rocketView, 
            soundManager, 
            combatService
        );

        ramEffect = new RamEffect(
            combatService,
            soundManager
        );

        weaponController = new WeaponController(
            weaponView,
            rocketController,
            projectileController,
            combatService
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
            combatService,
            bodySimulator,
            infantryView
        );

        armorController = new ArmorController(
            combatService,
            weaponController,
            motorVehicleController,
            ramEffect
        );

        platformController = new PlatformController(
            combatService,
            towableVehicleController,
            weaponController,
            ramEffect
        );

        driverController = new DriverController(
            combatService,
            motorVehicleController,
            ramEffect
        );
        
        rewardController = new RewardController(
            rewardView,
            rewardsMediator,
            infantryController,
            armorController
        );

        spawnSystem = new SpawnSystem(
            infantryController,
            armorController,
            maxVehicelCount,
            unitsCount
        );

        armorAIController = new ArmorAIController(
            combatService,
            pathfindingService,
            armorController,
            motorVehicleController,
            weaponController
        );

        behaviorSystem = new BehaviorSystem(
            navigationSystem,
            infantryController,
            combatService
        );

        commanderSystem = new CommanderSystem(
            infantryController,
            behaviorSystem
        );

        couplingController = new CouplingController(
            motorVehicleController,
            towableVehicleController
        );

        playerController = new PlayerController(
            playerView, 
            new PlayerInput(),
            playerConfig,
            physicsService,
            combatService,
            cameraManager,
            rewardController,
            weaponController,
            platformController,
            driverController,
            couplingController
        );

        enemyController = new EnemyController(
            unitView,
            spawnSystem,
            targetPoint,
            armorAIController,
            commanderSystem,
            navigationSystem
        );
    }

    private void Init() {
        playerController.Init();
    }

    private void Update() {
        combatService.UpdateSpatialTree();

        ramEffect.Update();
        bodySimulator.Update();
        navigationSystem.Update();
        spawnSystem.Update();

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
        commanderSystem.Update();
        behaviorSystem.Update();

        enemyController.Update();
        playerController.Update();

        rewardController.Update();
    }

}
using Codice.Client.Common.GameUI;

using Unity.Profiling;

using UnityEngine;
using UnityEngine.UIElements;

public class GameBootstrapper : MonoBehaviour {
    
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private CameraManager cameraManager;
    [Space]
    [SerializeField] private string physicsServiceLayer;
    [SerializeField] private string combatServiceLayer;
    [SerializeField] private string foeCombatServiceLayer;
    [SerializeField] private string vehicleObstacleLayer;
    [SerializeField] private string physicsObstacleLayer;
    [SerializeField] private LayerMask combatServiceEnvironmentMask;
    [Space]
    [SerializeField] private EnemyConfig enemyConfig;
    [SerializeField] private PlayerConfig playerConfig;
    [Space]
    [SerializeField] private AimVisuals aimVisualsPrefab;
    [SerializeField] private UIDocument uiDocument;
    [Space]
    [SerializeField] private string rewardsLayerName; // FIXME: not used
    [SerializeField] private GameObject rewardVisualsPrefab;

    private CombatSystem combatSystem;
    private PlayerController playerController;
    private EnemyController enemyController;
    private RewardController rewardController;
    private ProjectileController projectileController;
    private RocketController rocketController;
    private WeaponController weaponController;
    private RamEffect ramEffect;
    private InfantryController infantryController;
    private ArmorController armorController;
    private PlatformController platformController;
    private TruckController truckController;
    private SpawnService spawnService;
    private CommanderController commanderController;
    private ArmorAIController armorAIController;
    private SquadAIController squadAIController;
    private ProductionBuildingController buildingController;
    private HeadquarterBuildingController headquarterBuildingController;
    private ProductionSpaceController productionSpaceController;

    private void Start() {
        Build();
        Init();
    }

    private void Build() {
        var vehicleService = new VehicleService(LayerMask.NameToLayer(vehicleObstacleLayer));
        var localAvoidanceService = new LocalAvoidanceService();
        var pathfindingService = new PathfindingService(FlowFieldSystem.Instance);
        var physicsService = new PhysicsService(
            container: null, 
            operationalLayer: LayerMask.NameToLayer(physicsServiceLayer), 
            obstacleLayer: LayerMask.NameToLayer(physicsObstacleLayer)
        );

        var playerView = new PlayerView(uiDocument, aimVisualsPrefab);
        var unitView = new EnemyView();
        var weaponView = new WeaponView();
        var armorView = new ArmorView(soundManager);
        var platformView = new PlatformView();
        var rewardView = new RewardView(rewardVisualsPrefab);
        var infantryView = new InfantryView();
        var truckView = new TruckView(soundManager);
        var rocketView = new RocketView();
        var projectileView = new ProjectileView();
        var productionBuildingView = new ProductionBuildingView();
        
        combatSystem = new CombatSystem(
            LayerMask.NameToLayer(combatServiceLayer), 
            LayerMask.NameToLayer(foeCombatServiceLayer), 
            combatServiceEnvironmentMask
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

        infantryController = new InfantryController(
            combatSystem,
            infantryView,
            rewardController,
            physicsService,
            localAvoidanceService
        );

        armorController = new ArmorController(
            combatSystem,
            weaponController,
            vehicleService,
            ramEffect,
            rewardController,
            armorView
        );

        platformController = new PlatformController(
            combatSystem,
            weaponController,
            ramEffect,
            vehicleService,
            platformView
        );

        truckController = new TruckController(
            combatSystem,
            ramEffect,
            truckView,
            vehicleService
        );

        spawnService = new SpawnService(
            infantryController,
            armorController
        );

        armorAIController = new ArmorAIController(
            combatSystem,
            pathfindingService,
            armorController,
            weaponController
        );

        squadAIController = new SquadAIController(
            infantryController,
            pathfindingService,
            combatSystem
        );

        buildingController = new ProductionBuildingController(
            productionBuildingView,
            combatSystem,
            vehicleService,
            physicsService,
            localAvoidanceService,
            spawnService
        );

        headquarterBuildingController = new HeadquarterBuildingController(
            combatSystem,
            pathfindingService,
            vehicleService,
            physicsService,
            localAvoidanceService
        );

        productionSpaceController = new ProductionSpaceController(
            spawnService
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
            truckController,
            headquarterBuildingController
        );

        var producerFactory = new ProducerFactory(
            buildingController,
            productionSpaceController
        );

        commanderController = new CommanderController(
            squadAIController,
            armorAIController,
            producerFactory
        );

        enemyController = new EnemyController(
            unitView,
            enemyConfig,
            commanderController,
            buildingController,
            productionSpaceController
        );
    }

    private void Init() {
        enemyController.Init();
        playerController.Init();
    }

    private void Update() {
        combatSystem.Update();

        ramEffect.Update();
        rewardController.Update();

        projectileController.Update();
        rocketController.Update();
        weaponController.Update();
        infantryController.Update();
        armorController.Update();
        platformController.Update();
        truckController.Update();
        
        armorAIController.Update();
        squadAIController.Update();

        buildingController.Update();
        headquarterBuildingController.Update();
        productionSpaceController.Update();

        commanderController.Update();
        enemyController.Update();
        playerController.Update();

    }

    private void OnDestroy() {
        combatSystem.Destroy();
        rewardController.Destroy();       
    }

}

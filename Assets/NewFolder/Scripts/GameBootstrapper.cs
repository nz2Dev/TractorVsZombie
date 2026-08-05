using Codice.Client.Common.GameUI;

using Unity.Profiling;

using UnityEngine;
using UnityEngine.UIElements;

public class GameBootstrapper : MonoBehaviour {
    
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private CameraManager cameraManager;
    [Space]
    [SerializeField] private string physicsServiceLayer;
    [SerializeField] private string combatServiceLayer;
    [SerializeField] private string foeCombatServiceLayer;
    [SerializeField] private string vehicleObstacleLayer;
    [SerializeField] private string physicsObstacleLayer;
    [SerializeField] private LayerMask combatServiceEnvironmentMask;

    private CombatSystem combatSystem;
    private PlayerController playerController;
    private EnemyController enemyController;
    private LevelController levelController;
    private RewardController rewardController;
    private ProjectileController projectileController;
    private RocketController rocketController;
    private WeaponController weaponController;
    private RamEffectController ramEffect;
    private InfantryController infantryController;
    private LoadoutController loadoutController;
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
        var cameraProvider = new CameraProvider(cameraManager);
        var vehicleService = new VehicleService(LayerMask.NameToLayer(vehicleObstacleLayer));
        var localAvoidanceService = new LocalAvoidanceService();
        var pathfindingService = new PathfindingService(FlowFieldSystem.Instance);
        var physicsService = new PhysicsService(
            container: null, 
            operationalLayer: LayerMask.NameToLayer(physicsServiceLayer), 
            obstacleLayer: LayerMask.NameToLayer(physicsObstacleLayer)
        );

        var weaponView = new WeaponView();
        var armorView = new ArmorView(soundManager);
        var platformView = new PlatformView();
        var rewardView = new RewardView();
        var infantryView = new InfantryView();
        var truckView = new TruckView(soundManager);
        var rocketView = new RocketView(soundManager);
        var projectileView = new ProjectileView(soundManager);
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
            combatSystem
        );

        ramEffect = new RamEffectController(
            new RamEffectView(soundManager),
            combatSystem
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

        loadoutController = new LoadoutController(
            new LoadoutView(),
            weaponController
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
            loadoutController,
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
            new DrivingController(truckController),
            new AssemblingController(platformController, truckController),
            new SelectingController(new SelectingView(uiDocument), platformController),
            new AimingController(new AimingView(), cameraProvider, physicsService, combatSystem, platformController, weaponController),
            new CollectingController(rewardController),
            new CameraController(new CameraView(cameraManager))
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
            commanderController,
            buildingController,
            productionSpaceController
        );

        levelController = new LevelController(
            new LevelView(cameraManager),
            playerController,
            enemyController,
            headquarterBuildingController,
            cameraProvider
        );
    }

    private void Init() {
        levelController.Init(GameObject.FindFirstObjectByType<LevelPrototypeSource>().Get());
    }

    private void Update() {
        combatSystem.Update();

        ramEffect.Update();
        rewardController.Update();

        projectileController.Update();
        rocketController.Update();
        weaponController.Update();

        loadoutController.Update();
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

        levelController.Update();
    }

    private void OnDestroy() {
        combatSystem.Destroy();
        rewardController.Destroy();       
    }

}

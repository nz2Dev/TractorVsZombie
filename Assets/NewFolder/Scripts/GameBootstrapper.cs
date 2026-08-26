using Combat;

using UnityEngine;
using UnityEngine.UIElements;

public class GameBootstrapper : MonoBehaviour {

    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private RaycastConfig raycastConfig;
    [SerializeField] private KnnRunner knnRunner;
    [SerializeField] private FootstepSoundSystem footstepSoundSystem;
    [Space]
    [SerializeField] private string vehicleObstacleLayer;

    private CombatSystem combatSystem;
    private InteractionRegistry interactionRegistry;
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

    private void Awake() {
        if (!NonNullValidator.ValidateScene()) {
            Debug.LogError("Game initialization aborted because the scene is invalid.", this);
            enabled = false;
        }
    }

    private void Start() {
        Build();
        Init();
    }

    private void Build() {
        var vehicleService = new VehicleService(LayerMask.NameToLayer(vehicleObstacleLayer));
        var localAvoidanceService = new LocalAvoidanceService();
        var pathfindingService = new PathfindingService(FlowFieldSystem.Instance);
        var physicsService = new RagdollService();
        var raycastService = new RaycastService(raycastConfig);
        var proximityService = new ProximityService(knnRunner);

        var weaponView = new WeaponView();
        var armorView = new ArmorView(soundManager);
        var platformView = new PlatformView();
        var rewardView = new RewardView();
        var infantryView = new InfantryView(footstepSoundSystem);
        var truckView = new TruckView(soundManager);
        var rocketView = new RocketView(soundManager);
        var projectileView = new ProjectileView(soundManager);
        var productionBuildingView = new ProductionBuildingView();

        var entityMapping = new EntityMapping();

        combatSystem = new CombatSystem();
        interactionRegistry = new InteractionRegistry();

        rewardController = new RewardController(
            rewardView
        );

        infantryController = new InfantryController(
            combatSystem,
            infantryView,
            rewardController,
            physicsService,
            raycastService,
            localAvoidanceService,
            proximityService,
            interactionRegistry,
            entityMapping
        );

        rocketController = new RocketController(
            rocketView,
            combatSystem,
            raycastService,
            interactionRegistry,
            entityMapping
        );

        projectileController = new ProjectileController(
            combatSystem,
            projectileView,
            raycastService,
            entityMapping
        );

        weaponController = new WeaponController(
            weaponView,
            rocketController,
            projectileController
        );

        // rocket controller used to be initialized before infantry controller

        ramEffect = new RamEffectController(
            new RamEffectView(soundManager),
            combatSystem,
            raycastService,
            interactionRegistry,
            entityMapping
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
            armorView,
            proximityService,
            raycastService,
            entityMapping
        );

        platformController = new PlatformController(
            combatSystem,
            loadoutController,
            ramEffect,
            vehicleService,
            platformView,
            proximityService,
            raycastService,
            entityMapping
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
            weaponController,
            proximityService
        );

        squadAIController = new SquadAIController(
            infantryController,
            pathfindingService,
            combatSystem,
            proximityService,
            entityMapping
        );

        buildingController = new ProductionBuildingController(
            productionBuildingView,
            combatSystem,
            vehicleService,
            physicsService,
            localAvoidanceService,
            spawnService,
            proximityService,
            raycastService,
            entityMapping
        );

        headquarterBuildingController = new HeadquarterBuildingController(
            combatSystem,
            pathfindingService,
            vehicleService,
            physicsService,
            localAvoidanceService,
            raycastService,
            entityMapping,
            proximityService
        );

        productionSpaceController = new ProductionSpaceController(
            spawnService
        );

        playerController = new PlayerController(
            new DrivingController(truckController),
            new AssemblingController(new AssemblingView(), platformController, truckController),
            new SelectingController(new SelectingView(uiDocument), platformController),
            new AimingController(new AimingView(cameraManager), raycastService, combatSystem, platformController, weaponController, proximityService),
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
            headquarterBuildingController
        );
    }

    private void Init() {
        levelController.Init(GameObject.FindFirstObjectByType<LevelPrototypeSource>().Get());
    }

    private void Update() {
        combatSystem.Update();
        interactionRegistry.Update();

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
        rewardController.Destroy();
    }

}

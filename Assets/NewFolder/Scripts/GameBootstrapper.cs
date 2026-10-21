
using UnityEngine;
using UnityEngine.UIElements;

public class GameBootstrapper : MonoBehaviour {

    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private CameraManager cameraManager;
    [Inline, SerializeField] private RaycastServiceConfig raycastConfig;
    [Inline, SerializeField] private CollisionServiceConfig collisionServiceConfig;
    [SerializeField] private KnnRunner knnRunner;
    [SerializeField] private FootstepSoundSystem footstepSoundSystem;
    
    private HealthSystem healthSystem;
    private FactionRegistry factionRegistry;
    private HitboxSystem hitboxSystem;
    private MotionSystem motionSystem;
    private VehicleSystem vehicleSystem;
    private MovementSystem movementSystem;
    private PlayerController playerController;
    private EnemyController enemyController;
    private LevelController levelController;
    private RewardController rewardController;
    private DropSystem dropSystem;
    private AttachmentSystem attachmentSystem;
    private ProjectileController projectileController;
    private RocketController rocketController;
    private WeaponController weaponController;
    private FormationController formationController;
    private RamSystem ramEffect;
    private InfantryController infantryController;
    private LoadoutController loadoutController;
    private ArmorController armorController;
    private PlatformController platformController;
    private TruckController truckController;
    private SpawnerController spawnerController;
    private ProductionBuildingController buildingController;
    private HeadquarterBuildingController headquarterBuildingController;

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
        var vehicleService = new VehicleService();
        var localAvoidanceService = new AvoidanceService();
        var pathfindingService = new PathfindingService(FlowFieldSystem.Instance);
        var physicsService = new RagdollService();
        var raycastService = new RaycastService(raycastConfig);
        var proximityService = new ProximityService(knnRunner);
        var collisionService = new CollisionService(collisionServiceConfig);

        var weaponView = new WeaponView();
        var platformView = new PlatformView(cameraManager);
        var rewardView = new RewardView();
        var infantryView = new InfantryView(footstepSoundSystem);
        var rocketView = new RocketView(soundManager);
        var projectileView = new ProjectileView(soundManager);
        var productionBuildingView = new ProductionBuildingView();

        var entityMapping = new EntityMapping();
        var poseRegistry = new PoseRegistry();

        healthSystem = new HealthSystem();
        factionRegistry = new FactionRegistry();
        hitboxSystem = new HitboxSystem(factionRegistry, poseRegistry, raycastService, proximityService);
        vehicleSystem = new VehicleSystem(vehicleService, poseRegistry);
        motionSystem = new MotionSystem(physicsService, collisionService, healthSystem, poseRegistry);
        movementSystem = new MovementSystem(motionSystem, poseRegistry, localAvoidanceService);

        rewardController = new RewardController(
            rewardView
        );

        dropSystem = new DropSystem(healthSystem, poseRegistry, rewardController);

        infantryController = new InfantryController(
            healthSystem,
            factionRegistry,
            infantryView,
            dropSystem,
            motionSystem,
            movementSystem,
            hitboxSystem,
            entityMapping,
            poseRegistry
        );

        rocketController = new RocketController(
            rocketView,
            healthSystem,
            factionRegistry,
            hitboxSystem,
            motionSystem,
            entityMapping
        );

        projectileController = new ProjectileController(
            healthSystem,
            factionRegistry,
            projectileView,
            hitboxSystem,
            entityMapping,
            collisionService
        );

        weaponController = new WeaponController(
            weaponView,
            rocketController,
            projectileController
        );

        attachmentSystem = new AttachmentSystem(poseRegistry, weaponController);

        // rocket controller used to be initialized before infantry controller

        formationController = new FormationController(
            infantryController
        );

        ramEffect = new RamSystem(
            healthSystem,
            factionRegistry,
            hitboxSystem,
            motionSystem,
            entityMapping,
            vehicleSystem
        );

        loadoutController = new LoadoutController(
            new LoadoutView(),
            weaponController
        );

        armorController = new ArmorController(
            healthSystem,
            factionRegistry,
            attachmentSystem,
            vehicleSystem,
            ramEffect,
            dropSystem,
            new ArmorPresenter(
                new ArmorAudio(soundManager),
                new ArmorView(),
                new ArmorUI(cameraManager)
            ),
            hitboxSystem,
            entityMapping,
            poseRegistry
        );

        platformController = new PlatformController(
            healthSystem,
            factionRegistry,
            loadoutController,
            vehicleSystem,
            platformView,
            hitboxSystem,
            entityMapping,
            poseRegistry
        );

        truckController = new TruckController(
            healthSystem,
            factionRegistry,
            ramEffect,
            new TruckPresenter(
                new TruckAudio(soundManager),
                new TruckView(),
                new TruckUI(cameraManager)
            ),
            vehicleSystem,
            hitboxSystem,
            entityMapping,
            poseRegistry
        );

        var spawnService = new SpawnService(infantryController, armorController);

        spawnerController = new SpawnerController(
            spawnService
        );

        buildingController = new ProductionBuildingController(
            productionBuildingView,
            healthSystem,
            factionRegistry,
            collisionService,
            localAvoidanceService,
            spawnerController,
            hitboxSystem,
            entityMapping
        );

        headquarterBuildingController = new HeadquarterBuildingController(
            healthSystem,
            factionRegistry,
            pathfindingService,
            collisionService,
            localAvoidanceService,
            hitboxSystem,
            entityMapping,
            new HeadquarterBuildingView(cameraManager)
        );

        playerController = new PlayerController(
            new DrivingController(truckController, vehicleSystem, platformController),
            new AssemblingController(new AssemblingView(), platformController, truckController, vehicleSystem),
            new SelectingController(new SelectingView(uiDocument), platformController),
            new AimingController(new AimingView(cameraManager), platformController, weaponController, hitboxSystem, collisionService),
            new CollectingController(rewardController),
            new CameraController(new CameraView(cameraManager))
        );

        enemyController = new EnemyController(
            new InfantryAIController(infantryController, pathfindingService, hitboxSystem, entityMapping, formationController),
            new ArmorAIController(healthSystem, pathfindingService, armorController, vehicleSystem, weaponController, hitboxSystem),
            new ProductionController(new ProducerFactory(buildingController, spawnerController), pathfindingService),
            new GoalsController(pathfindingService, platformController, truckController),
            new SquadsService(formationController, infantryController)
        );

        levelController = new LevelController(
            new LevelView(cameraManager, uiDocument),
            playerController,
            enemyController,
            headquarterBuildingController
        );
    }

    private void Init() {
        levelController.Init(GameObject.FindFirstObjectByType<LevelPrototypeSource>().Get());
    }

    private void Update() {
        healthSystem.Update();
        vehicleSystem.Update();
        attachmentSystem.Update();
        motionSystem.Update();
        movementSystem.Update();
        hitboxSystem.Update();
        dropSystem.Update();

        ramEffect.Update();
        rewardController.Update();

        projectileController.Update();
        rocketController.Update();
        weaponController.Update();

        spawnerController.Update();
        loadoutController.Update();
        infantryController.Update();
        armorController.Update();
        platformController.Update();
        truckController.Update();

        formationController.Update();
        buildingController.Update();
        headquarterBuildingController.Update();

        enemyController.Update();
        playerController.Update();

        levelController.Update();
    }

    private void OnDestroy() {
        rewardController.Destroy();
    }

}

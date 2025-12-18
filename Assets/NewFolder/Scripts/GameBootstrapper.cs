using Codice.Client.Common.GameUI;

using Unity.Profiling;

using UnityEngine;
using UnityEngine.UIElements;

public class GameBootstrapper : MonoBehaviour {
    
    [SerializeField] private RocketView rocketView; // todo remove monobehaviour inheritance
    [SerializeField] private ProjectileView projectileView;
    [Space]
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private FlowFieldsSurface flowFieldsSurface;
    [SerializeField] private ORCAEnvironment orcaEnvironment;
    [Space]
    [SerializeField] private string physicsServiceLayer;
    [SerializeField] private string combatServiceLayer;
    [SerializeField] private LayerMask combatServiceEnvironmentMask;
    [Space]
    [SerializeField] private InfantryConfig enemyInfantryConfig;
    [SerializeField] int unitsCount = 10;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] Transform targetPoint;
    [Space]
    [SerializeField] private int maxVehicelCount = 10;
    [SerializeField] private ArmorConfig enemyArmorConfig;
    [Space]
    [SerializeField] private PlayerConfig playerConfig;
    [SerializeField] private AimVisuals aimVisualsPrefab;
    [SerializeField] private UIDocument uiDocument;
    [Space]
    [SerializeField] private string rewardsLayerName;
    [SerializeField] private GameObject rewardVisualsPrefab;

    private CombatService combatService;

    private PlayerController playerController;
    private EnemyController enemyController;
    private RewardController rewardController;
    private ProjectileController projectileController;
    private RocketController rocketController;
    private WeaponController weaponController;
    private MotorVehicleController motorVehicleController;
    private TowableVehicleController towableVehicleController;
    private InfantryController infantryController;
    private ArmorController armorController;
    private PlatformController platformController;
    private DriverController driverController;
    private ArmorAIController armorAIController;
    private CouplingController couplingController;

    private void Start() {
        Build();
        Init();
    }

    private void Build() {
        combatService = new CombatService(LayerMask.NameToLayer(combatServiceLayer), combatServiceEnvironmentMask);
        var vehicleService = new VehicleService();
        var localAvoidanceService = new LocalAvoidanceService(orcaEnvironment);
        var navigationService = new NavigationService(flowFieldsSurface);
        var physicsService = new PhysicsService(container: null, LayerMask.NameToLayer(physicsServiceLayer));
        var rewardsMediator = new RewardsMediator(LayerMask.NameToLayer(rewardsLayerName));

        var playerView = new PlayerView(uiDocument, aimVisualsPrefab);
        var unitView = new EnemyView();
        var weaponView = new WeaponView();
        var motorVehicleView = new MotorVehicleView();
        var towableVehicleView = new TowableVehicleView();
        var rewardView = new RewardView(rewardVisualsPrefab);
        var infantryView = new InfantryView();

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

        weaponController = new WeaponController(
            weaponView,
            rocketController,
            projectileController,
            combatService
        );

        motorVehicleController = new MotorVehicleController(
            motorVehicleView,
            vehicleService,
            combatService,
            soundManager
        );

        towableVehicleController = new TowableVehicleController(
            towableVehicleView,
            vehicleService
        );

        infantryController = new InfantryController(
            infantryView,
            combatService,
            navigationService,
            localAvoidanceService,
            physicsService
        );

        armorController = new ArmorController(
            combatService,
            weaponController,
            motorVehicleController
        );

        platformController = new PlatformController(
            combatService,
            towableVehicleController,
            weaponController
        );

        driverController = new DriverController(
            combatService,
            motorVehicleController
        );
        
        rewardController = new RewardController(
            rewardView,
            rewardsMediator,
            infantryController,
            armorController
        );

        armorAIController = new ArmorAIController(
            combatService,
            navigationService,
            armorController,
            motorVehicleController,
            weaponController
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
            spawnPoints,
            targetPoint,
            unitsCount,
            enemyInfantryConfig,
            infantryController,
            maxVehicelCount,
            enemyArmorConfig,
            armorController,
            armorAIController
        );
    }

    private void Init() {
        playerController.Init();
    }

    private void Update() {
        combatService.UpdateSpatialTree();

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

        enemyController.Update();
        playerController.Update();

        rewardController.Update();
    }

}
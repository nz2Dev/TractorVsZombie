using Codice.Client.Common.GameUI;

using Unity.Profiling;

using UnityEngine;

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
    private VehicleController vehicleController;
    private InfantryController infantryController;
    private ArmorController armorController;
    private PlatformController platformController;

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

        var playerView = new PlayerView();
        var unitView = new EnemyView();
        var weaponView = new WeaponView();
        var vehicleView = new VehicleView();
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

        vehicleController = new VehicleController(
            vehicleService,
            soundManager,
            vehicleView
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
            navigationService,
            weaponController,
            vehicleController
        );

        platformController = new PlatformController(
            combatService,
            vehicleController,
            weaponController
        );

        playerController = new PlayerController(
            playerView, 
            combatService,
            cameraManager,
            soundManager,
            playerConfig,
            vehicleController,
            platformController
        );

        enemyController = new EnemyController(
            unitView,
            navigationService,
            spawnPoints,
            targetPoint,
            unitsCount,
            enemyInfantryConfig,
            infantryController,
            maxVehicelCount,
            enemyArmorConfig,
            armorController,
            vehicleController
        );

        rewardController = new RewardController(
            rewardView,
            rewardsMediator,
            playerController,
            infantryController,
            armorController
        );
    }

    private void Init() {
        playerController.Init();
        enemyController.Init();
    }

    private void Update() {
        combatService.UpdateSpatialTree();

        projectileController.Update();
        rocketController.Update();
        weaponController.Update();
        vehicleController.Update();
        infantryController.Update();
        armorController.Update();
        platformController.Update();
        
        enemyController.Update();
        playerController.Update();

        rewardController.Update();
    }

}
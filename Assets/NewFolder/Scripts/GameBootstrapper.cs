using Codice.Client.Common.GameUI;

using Unity.Profiling;

using UnityEngine;

public class GameBootstrapper : MonoBehaviour {
    
    [SerializeField] private RocketView rocketView; // todo remove monobehaviour inheritance
    [SerializeField] private ParticleSystem bulletSystemPrefab;
    [Space]
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private CameraManager cameraManager;
    [Space]
    [SerializeField] private string physicsServiceLayer;
    [SerializeField] private string combatServiceLayer;
    [SerializeField] private LayerMask combatServiceEnvironmentMask;
    [Space]
    [SerializeField] private bool enemyComponent = true; // todo remove
    [SerializeField] private InfantryConfig enemyInfantryConfig;
    [SerializeField] int unitsCount = 10;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] Transform targetPoint;
    [SerializeField] private FlowFieldsSurface flowFieldsSurface;
    [SerializeField] private ORCAEnvironment orcaEnvironment;
    [SerializeField] private UnitVisuals unitVisualsPrefab;
    [Space]
    [SerializeField] private int maxVehicelCount = 10;
    [SerializeField] private ArmorConfig enemyArmorConfig;
    [Space]
    [SerializeField] private bool playerComponent = true; // todo remove
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

    private void Start() {
        var vehicleService = new VehicleService();
        var localAvoidanceService = new LocalAvoidanceService(orcaEnvironment);
        var navigationService = new NavigationService(flowFieldsSurface);
        var physicsService = new PhysicsService(container: null, LayerMask.NameToLayer(physicsServiceLayer));
        combatService = new CombatService(LayerMask.NameToLayer(combatServiceLayer), combatServiceEnvironmentMask);
        var rewardsMediator = new RewardsMediator(LayerMask.NameToLayer(rewardsLayerName));

        var playerView = new PlayerView();
        var unitView = new EnemyView();
        var projectileView = new ProjectileView(bulletSystemPrefab);
        var weaponView = new WeaponView();
        var vehicleView = new VehicleView();
        var rewardView = new RewardView(rewardVisualsPrefab);
        var infantryView = new InfantryView();

        projectileController = new ProjectileController(
            combatService,
            soundManager,
            projectileView
        );
        projectileController.Init(); // todo remove

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

        playerController = new PlayerController(
            playerView, 
            combatService,
            cameraManager,
            soundManager,
            weaponController,
            playerConfig,
            vehicleController
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

        if (playerComponent) playerController.Init();
        if (enemyComponent) enemyController.Init();
    }

    private static readonly ProfilerMarker cameraUpdateMarker = new ProfilerMarker("Game.CameraController");
    private static readonly ProfilerMarker unitUpdateMarker = new ProfilerMarker("Game.UnitController");
    private static readonly ProfilerMarker playerUpdateMarker = new ProfilerMarker("Game.PlayerController");

    private void Update() {
        combatService.UpdateSpatialTree();

        projectileController.Update();
        rocketController.Update();
        weaponController.Update();
        vehicleController.Update();
        infantryController.Update();
        armorController.Update();
        
        if (enemyComponent) 
            using (unitUpdateMarker.Auto()) 
                enemyController.Update();

        if (playerComponent)
            using (playerUpdateMarker.Auto())
                playerController.Update();

        rewardController.Update();
    }

}
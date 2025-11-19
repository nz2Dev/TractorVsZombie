using Codice.Client.Common.GameUI;

using Unity.Profiling;

using UnityEngine;

public class GameBootstrapper : MonoBehaviour {
    
    [SerializeField] private ParticleSystem bulletSystemPrefab;
    [Space]
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private CameraManager cameraManager;
    [Space]
    [SerializeField] private string physicsServiceLayer;
    [SerializeField] private string combatServiceLayer;
    [SerializeField] private LayerMask combatServiceEnvironmentMask;
    [Space]
    [SerializeField] private bool enemyComponent = true;
    [SerializeField] int unitsCount = 10;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] Transform targetPoint;
    [SerializeField] private FlowFieldsSurface flowFieldsSurface;
    [SerializeField] private ORCAEnvironment orcaEnvironment;
    [SerializeField] private UnitVisuals unitVisualsPrefab;
    [Space]
    [SerializeField] private int maxVehicelCount = 10;
    [SerializeField] private UnitVehicleData foeVehicle;
    [Space]
    [SerializeField] private bool playerComponent = true;
    [SerializeField] private DriverVehicleData driveVehicle;
    [SerializeField] private TrailerVehicleData trailerVehicle;
    [SerializeField] private int trailersCount = 3;
    [Space]
    [SerializeField] private ProjectileService projectileServiceImpl;
    [SerializeField] private TurelConfig turelData;
    [SerializeField] private RocketLauncherConfig rocketLauncherConfig;
    [Space]
    [SerializeField] private string rewardsLayerName;
    [SerializeField] private GameObject rewardVisualsPrefab;

    private CombatService combatService;

    private PlayerController playerController;
    private EnemyController enemyController;

    private ProjectileController projectileController;

    private void Start() {
        var vehicleService = new VehicleService();
        var localAvoidanceService = new LocalAvoidanceService(orcaEnvironment);
        var navigationService = new NavigationService(flowFieldsSurface);
        var physicsService = new PhysicsService(container: null, LayerMask.NameToLayer(physicsServiceLayer));
        combatService = new CombatService(LayerMask.NameToLayer(combatServiceLayer), combatServiceEnvironmentMask);
        var projectileService = projectileServiceImpl;
        var rewardsMediator = new RewardsMediator(LayerMask.NameToLayer(rewardsLayerName));

        var playerView = new PlayerView();
        var unitView = new EnemyView(unitVisualsPrefab, null);
        var projectileView = new ProjectileView(bulletSystemPrefab);

        projectileController = new ProjectileController(
            combatService,
            soundManager,
            projectileView
        );
        projectileController.Init();

        playerController = new PlayerController(
            vehicleService, playerView,
            driveVehicle, trailerVehicle,
            trailersCount,
            combatService, 
            
            combatService,
            turelData,
            rocketLauncherConfig,

            soundManager,
            cameraManager,
            rewardsMediator,
            
            projectileController);

        enemyController = new EnemyController(
            localAvoidanceService,
            navigationService,
            unitView,
            spawnPoints,
            targetPoint,
            unitsCount,
            combatService,
            physicsService,
            rewardsMediator,
            
            vehicleService,
            foeVehicle,
            maxVehicelCount,
            soundManager,
            projectileService,
            
            rewardVisualsPrefab);

        if (playerComponent) playerController.Init();
        if (enemyComponent) enemyController.Init();
    }

    private static readonly ProfilerMarker cameraUpdateMarker = new ProfilerMarker("Game.CameraController");
    private static readonly ProfilerMarker unitUpdateMarker = new ProfilerMarker("Game.UnitController");
    private static readonly ProfilerMarker playerUpdateMarker = new ProfilerMarker("Game.PlayerController");

    private void Update() {
        combatService.UpdateSpatialTree();

        projectileController.Update();
        
        if (enemyComponent) 
            using (unitUpdateMarker.Auto()) 
                enemyController.Update();

        if (playerComponent)
            using (playerUpdateMarker.Auto())
                playerController.Update();
    }

}
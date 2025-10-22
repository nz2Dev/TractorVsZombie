using Codice.Client.Common.GameUI;

using Unity.Profiling;

using UnityEngine;

public class GameBootstrapper : MonoBehaviour {
    
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private CameraManager cameraManager;
    [Space]
    [SerializeField] private string physicsServiceLayer;
    [SerializeField] private string combatServiceLayer;
    [SerializeField] private LayerMask combatServiceEnvironmentMask;
    [Space]
    [SerializeField] private bool unitsComponent = true;
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
    [SerializeField] private VehiclePhysicsRoot vehiclePhysicsRoot;
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
    private UnitController unitController;

    private void Start() {
        var vehicleService = new VehicleService(vehiclePhysicsRoot);
        var localAvoidanceService = new LocalAvoidanceService(orcaEnvironment);
        var navigationService = new NavigationService(flowFieldsSurface);
        var physicsService = new PhysicsService(container: null, LayerMask.NameToLayer(physicsServiceLayer));
        combatService = new CombatService(LayerMask.NameToLayer(combatServiceLayer), combatServiceEnvironmentMask);
        var projectileService = projectileServiceImpl;
        var rewardsMediator = new RewardsMediator(LayerMask.NameToLayer(rewardsLayerName));

        var vehicleView = new VehicleView(null);
        var unitView = new UnitView(unitVisualsPrefab, null);
        var weaponView = new WeaponView();
        var rewardsView = new RewardsView();

        playerController = new PlayerController(
            vehicleService, vehicleView,
            driveVehicle, trailerVehicle,
            trailersCount,
            combatService, 
            
            weaponView, 
            combatService,
            turelData,
            projectileService,
            rocketLauncherConfig,

            soundManager,
            cameraManager,
            
            rewardsView,
            rewardsMediator);

        var foeVehicelView = new VehicleView(null);
        unitController = new UnitController(
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
        if (unitsComponent) unitController.Init();
    }

    private static readonly ProfilerMarker cameraUpdateMarker = new ProfilerMarker("Game.CameraController");
    private static readonly ProfilerMarker unitUpdateMarker = new ProfilerMarker("Game.UnitController");
    private static readonly ProfilerMarker playerUpdateMarker = new ProfilerMarker("Game.PlayerController");

    private void Update() {
        combatService.UpdateSpatialTree();
        
        if (unitsComponent) 
            using (unitUpdateMarker.Auto()) 
                unitController.Update();

        if (playerComponent)
            using (playerUpdateMarker.Auto())
                playerController.Update();
    }

}
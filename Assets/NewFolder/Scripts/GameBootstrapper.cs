using Unity.Profiling;

using UnityEngine;

public class GameBootstrapper : MonoBehaviour {
    
    [SerializeField] private SoundManager soundManager;
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
    [SerializeField] private bool cameraComponent = true;
    [Space]
    [SerializeField] private bool playerComponent = true;
    [SerializeField] private VehiclePhysicsRoot vehiclePhysicsRoot;
    [SerializeField] private VehicleBlueprint driveVehicle;
    [SerializeField] private VehicleBlueprint trailerVehicle;
    [SerializeField] private int trailersCount = 3;
    [Space]
    [SerializeField] private ProjectileService projectileServiceImpl;
    [SerializeField] private TurelVisuals turelVisualsPrefab;
    [SerializeField] private TurelConfig turelData;
    [SerializeField] private RocketLauncherVisuals rocketLauncherVisualsPrefab;
    [SerializeField] private RocketLauncherConfig rocketLauncherConfig;

    private CombatService combatService;

    private PlayerController playerController;
    private CameraController cameraController;
    private UnitController unitController;

    private void Start() {
        var vehicleService = new VehicleService(vehiclePhysicsRoot);
        var cameraService = new CameraService(Camera.main);
        var localAvoidanceService = new LocalAvoidanceService(orcaEnvironment);
        var navigationService = new NavigationService(flowFieldsSurface);
        var physicsService = new PhysicsService(container: null, LayerMask.NameToLayer(physicsServiceLayer));
        combatService = new CombatService(LayerMask.NameToLayer(combatServiceLayer), combatServiceEnvironmentMask);
        var projectileService = projectileServiceImpl;

        var vehicleView = new VehicleView(null);
        var unitView = new UnitView(unitVisualsPrefab);
        var weaponView = new WeaponView(turelVisualsPrefab, rocketLauncherVisualsPrefab);

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
            soundManager);
        
        cameraController = new CameraController(
            cameraService, vehicleService);

        unitController = new UnitController(
            localAvoidanceService,
            navigationService,
            unitView,
            spawnPoints,
            targetPoint,
            unitsCount,
            combatService,
            physicsService);

        if (playerComponent) playerController.Init();
        if (unitsComponent) unitController.Init();
        if (cameraComponent) cameraController.Init();
    }

    private static readonly ProfilerMarker cameraUpdateMarker = new ProfilerMarker("Game.CameraController");
    private static readonly ProfilerMarker unitUpdateMarker = new ProfilerMarker("Game.UnitController");
    private static readonly ProfilerMarker playerUpdateMarker = new ProfilerMarker("Game.PlayerController");

    private void Update() {
        combatService.UpdateSpatialTree();
        
        if (cameraComponent)
            using (cameraUpdateMarker.Auto())
                cameraController.Update();
        
        if (unitsComponent) 
            using (unitUpdateMarker.Auto()) 
                unitController.Update();

        if (playerComponent)
            using (playerUpdateMarker.Auto())
                playerController.Update();
    }

}
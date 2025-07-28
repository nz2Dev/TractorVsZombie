using System.Collections;

using Unity.Profiling;

using UnityEngine;

public class GameBootstrapper : MonoBehaviour {
    
    [SerializeField] private string combatServiceLayer;
    [SerializeField] private string vehicleServiceLayer;
    [SerializeField] private string physicsServiceLayer;
    [SerializeField] int unitsCount = 10;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] Transform targetPoint;
    [SerializeField] private FlowFieldsSurface flowFieldsSurface;
    [SerializeField] private ORCAEnvironment orcaEnvironment;
    [SerializeField] private GameObject unitVisualsPrefab;
    [Space]
    [SerializeField] private VehicleEntity driveVehicle;
    [SerializeField] private VehicleEntity trailerVehicle;
    [SerializeField] private int trailersCount = 3;

    private VehiclesController vehiclesController;
    private CameraController cameraController;
    private UnitController unitController;

    private void Start() {
        var vehicleService = new VehicleService(null, LayerMask.NameToLayer(vehicleServiceLayer));
        var vehicleView = new VehicleView(null);
        var cameraService = new CameraService(Camera.main);
        var localAvoidanceService = new LocalAvoidanceService(orcaEnvironment);
        var navigationService = new NavigationService(flowFieldsSurface);
        var physicsService = new PhysicsService(container: null, LayerMask.NameToLayer(physicsServiceLayer));
        var combatService = new CombatService(LayerMask.NameToLayer(combatServiceLayer));
        var unitView = new UnitView(unitVisualsPrefab);

        vehiclesController = new VehiclesController(
            vehicleService, vehicleView,
            driveVehicle, trailerVehicle,
            trailersCount,
            combatService);
        
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

        unitController.Init();
        vehiclesController.Init();
        cameraController.Init();
    }

    private static readonly ProfilerMarker cameraUpdateMarker = new ProfilerMarker("Game.CameraController");
    private static readonly ProfilerMarker unitUpdateMarker = new ProfilerMarker("Game.UnitController");
    private static readonly ProfilerMarker vehicleUpdateMarker = new ProfilerMarker("Game.VehicleController");


    private void Update() {
        using (cameraUpdateMarker.Auto())
            cameraController.Update();
        using (unitUpdateMarker.Auto())
            unitController.Update();
        using (vehicleUpdateMarker.Auto())
            vehiclesController.Update();
    }

    private void FixedUpdate() {
        vehiclesController.FixedUpdate();
    }
}
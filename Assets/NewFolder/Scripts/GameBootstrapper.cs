using System.Collections;

using UnityEngine;

public class GameBootstrapper : MonoBehaviour {
    
    [SerializeField] private string combatServiceLayer;
    [SerializeField] int unitsCount = 10;
    [SerializeField] Transform spawnPoint;
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

    private IEnumerator Start() {
        var vehicleService = new VehicleService(null);
        var vehicleView = new VehicleView(null);
        var cameraService = new CameraService(Camera.main);
        var localAvoidanceService = new LocalAvoidanceService(orcaEnvironment);
        var navigationService = new NavigationService(flowFieldsSurface);
        var physicsService = new PhysicsService(container: null);
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
            spawnPoint,
            targetPoint,
            unitsCount,
            combatService);

        vehiclesController.Init();
        cameraController.Init();
        yield return unitController.Initialize();
    }

    private void Update() {
        cameraController.Update();
        unitController.Update();
        vehiclesController.Update();
    }

    private void FixedUpdate() {
        vehiclesController.FixedUpdate();
    }
}
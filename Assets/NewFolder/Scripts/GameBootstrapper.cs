using System.Collections;

using UnityEngine;

public class GameBootstrapper : MonoBehaviour {
    
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
    private CrowdController crowdController;

    private IEnumerator Start() {
        var vehicleService = new VehicleService(null);
        var vehicleView = new VehicleView(null);
        var cameraService = new CameraService(Camera.main);
        var localAvoidanceService = new LocalAvoidanceService(orcaEnvironment);
        var navigationService = new NavigationService(flowFieldsSurface);
        var physicsService = new PhysicsService(parent: null);
        var crowdView = new CrowdView(unitVisualsPrefab);

        vehiclesController = new VehiclesController(
            vehicleService, vehicleView,
            driveVehicle, trailerVehicle,
            trailersCount);
        
        cameraController = new CameraController(
            cameraService, vehicleService);

        crowdController = new CrowdController(
            localAvoidanceService,
            navigationService,
            physicsService,
            crowdView,
            spawnPoint,
            targetPoint,
            unitsCount);

        vehiclesController.Init();
        cameraController.Init();
        yield return crowdController.Initialize();
    }

    private void Update() {
        cameraController.Update();
        crowdController.Update();
        vehiclesController.Update();
    }

    private void FixedUpdate() {
        vehiclesController.FixedUpdate();
    }
}
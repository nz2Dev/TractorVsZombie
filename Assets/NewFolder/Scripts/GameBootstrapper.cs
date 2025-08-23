using System.Collections;

using Codice.Client.Common;

using Unity.Profiling;

using UnityEngine;

public class GameBootstrapper : MonoBehaviour {
    
    [SerializeField] private string combatServiceLayer;
    [SerializeField] private string physicsServiceLayer;
    [SerializeField] int unitsCount = 10;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] Transform targetPoint;
    [SerializeField] private FlowFieldsSurface flowFieldsSurface;
    [SerializeField] private ORCAEnvironment orcaEnvironment;
    [SerializeField] private VehiclePhysicsRoot vehiclePhysicsRoot;
    [SerializeField] private GameObject unitVisualsPrefab;
    [Space]
    [SerializeField] private VehicleBlueprint driveVehicle;
    [SerializeField] private VehicleBlueprint trailerVehicle;
    [SerializeField] private int trailersCount = 3;
    [Space]
    [SerializeField] private TurelVisuals turelVisualsPrefab;

    private VehiclesController vehiclesController;
    private CameraController cameraController;
    private UnitController unitController;
    private WeaponController weaponController;

    private void Start() {
        var vehicleService = new VehicleService(vehiclePhysicsRoot);
        var vehicleView = new VehicleView(null);
        var cameraService = new CameraService(Camera.main);
        var localAvoidanceService = new LocalAvoidanceService(orcaEnvironment);
        var navigationService = new NavigationService(flowFieldsSurface);
        var physicsService = new PhysicsService(container: null, LayerMask.NameToLayer(physicsServiceLayer));
        var combatService = new CombatService(LayerMask.NameToLayer(combatServiceLayer));
        var unitView = new UnitView(unitVisualsPrefab);
        var weaponView = new WeaponView(turelVisualsPrefab);

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

        weaponController = new WeaponController(
            weaponView, 
            combatService);

        unitController.Init();
        vehiclesController.Init();
        cameraController.Init();
        weaponController.Init();
    }

    private static readonly ProfilerMarker cameraUpdateMarker = new ProfilerMarker("Game.CameraController");
    private static readonly ProfilerMarker unitUpdateMarker = new ProfilerMarker("Game.UnitController");
    private static readonly ProfilerMarker vehicleUpdateMarker = new ProfilerMarker("Game.VehicleController");
    private static readonly ProfilerMarker weaponUpdateMarker = new ProfilerMarker("Game.WeaponController");
    private static readonly ProfilerMarker fixedWeaponUpdateMarker = new ProfilerMarker("Game.Fixed.WeaponController");

    private void FixedUpdate() {
        using (fixedWeaponUpdateMarker.Auto())
            weaponController.FixedUpdate();
    }

    private void Update() {
        using (cameraUpdateMarker.Auto())
            cameraController.Update();
        using (unitUpdateMarker.Auto())
            unitController.Update();
        using (vehicleUpdateMarker.Auto())
            vehiclesController.Update();
        using (weaponUpdateMarker.Auto())
            weaponController.Update();
    }

}
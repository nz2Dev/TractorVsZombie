using System.Collections;

using Codice.Client.Common;

using Unity.Profiling;

using UnityEngine;

public class GameBootstrapper : MonoBehaviour {
    
    [SerializeField] private string combatServiceLayer;
    [SerializeField] private string physicsServiceLayer;
    [Space]
    [SerializeField] private bool unitsComponent = true;
    [SerializeField] int unitsCount = 10;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] Transform targetPoint;
    [SerializeField] private FlowFieldsSurface flowFieldsSurface;
    [SerializeField] private ORCAEnvironment orcaEnvironment;
    [SerializeField] private GameObject unitVisualsPrefab;
    [Space]
    [SerializeField] private bool cameraComponent = true;
    [Space]
    [SerializeField] private bool vehicleComponent = true;
    [SerializeField] private VehiclePhysicsRoot vehiclePhysicsRoot;
    [SerializeField] private VehicleBlueprint driveVehicle;
    [SerializeField] private VehicleBlueprint trailerVehicle;
    [SerializeField] private int trailersCount = 3;
    [Space]
    [SerializeField] private bool weaponsComponent = true;
    [SerializeField] private TurelVisuals turelVisualsPrefab;
    [SerializeField] private TurelConfig turelData;

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
            combatService,
            turelData);

        if (unitsComponent) unitController.Init();
        if (vehicleComponent) vehiclesController.Init();
        if (weaponsComponent) weaponController.Init();
        if (cameraComponent) cameraController.Init();
    }

    private static readonly ProfilerMarker cameraUpdateMarker = new ProfilerMarker("Game.CameraController");
    private static readonly ProfilerMarker unitUpdateMarker = new ProfilerMarker("Game.UnitController");
    private static readonly ProfilerMarker vehicleUpdateMarker = new ProfilerMarker("Game.VehicleController");
    private static readonly ProfilerMarker weaponUpdateMarker = new ProfilerMarker("Game.WeaponController");

    private void Update() {
        if (cameraComponent)
            using (cameraUpdateMarker.Auto())
                cameraController.Update();
        
        if (unitsComponent) 
            using (unitUpdateMarker.Auto()) 
                unitController.Update();

        if (vehicleComponent)
            using (vehicleUpdateMarker.Auto())
                vehiclesController.Update();
        
        if (weaponsComponent)
            using (weaponUpdateMarker.Auto())
                weaponController.Update();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        if (weaponsComponent) weaponController?.OnDrawGizmos();
    }
#endif

}
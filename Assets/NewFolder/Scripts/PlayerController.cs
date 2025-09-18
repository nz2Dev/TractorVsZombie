using System.Collections.Generic;

using UnityEngine;

public class PlayerController {

    private readonly CombatService combatService;
    private readonly VehicleService vehicleService;
    private readonly ProjectileService projectileService;
    private readonly VehicleView vehicleView;
    private readonly WeaponView view;

    private readonly int trailersCount;
    private readonly VehicleBlueprint driveVehicleBlueprint;
    private readonly VehicleBlueprint trailerVehicleBlueprint;

    private readonly List<Vehicle> vehicles = new ();
    private Vehicle driveVehicle;
    private int driveVehicleCombatId;

    private readonly TurelConfig turelConfig;
    private readonly RocketLauncherConfig launcherConfig;
    
    private int turelIdCounter;
    private int turelsCombatGroupId;
    private readonly List<Turel> turels = new ();
    private readonly Dictionary<int, int> turelToCombatId = new ();
    private readonly Dictionary<int, Vehicle> turelToVehicle = new ();
    private readonly Dictionary<int, int> turelToProjectileGroupId = new ();
    private readonly List<ProjectileState> projectilesStateBuffer = new (64);

    private int rocketLauncherIdCounter;
    private readonly List<RocketLauncher> rocketLaunchers = new ();
    private readonly Dictionary<int, int> rocketLauncherToCombatId = new ();
    private readonly Dictionary<int, Vehicle> rocketLauncherToVehicle = new ();

    private int rocketIdCounter;
    private readonly Dictionary<int, List<Rocket>> rocketLauncherRocketsRegistry = new ();

    public PlayerController(VehicleService vehicleService, VehicleView vehicleView, VehicleBlueprint driveVehicle, VehicleBlueprint trailerVehicle, int trailersCount, 
        CombatService combatService, WeaponView weaponView, CombatService interactionService, TurelConfig turelConfig, ProjectileService projectileService, 
        RocketLauncherConfig launcherConfig) {
        this.view = weaponView;
        this.combatService = interactionService;
        this.turelConfig = turelConfig;
        this.projectileService = projectileService;
        this.launcherConfig = launcherConfig;
    
        this.vehicleService = vehicleService;
        this.vehicleView = vehicleView;
        this.driveVehicleBlueprint = driveVehicle;
        this.trailerVehicleBlueprint = trailerVehicle;
        this.trailersCount = trailersCount;
        this.combatService = combatService;
    }

    public void Init() {
        turelsCombatGroupId = combatService.AddGroup();
        SpawnDriveVehicle(Vector3.zero);

        for (int i = 0; i < trailersCount; i++) {
            SpawnTrailerVehicle(new Vector3(0, 0, -2f + i * -2f));
        }

        bool flipFlop = false;
        foreach (var vehicle in vehicles) {
            if (!vehicle.TowingTonqueRotation.HasValue)
                continue;

            if (flipFlop = !flipFlop) {
                SpawnTurel(vehicle, turelConfig);
            } else {
                SpawnRocketLauncher(vehicle, launcherConfig);
            }
        }
    }

    public void Update() {
        ReadVehiclesOrientation();
        ReadDriveVehicleInput();
        UpdateVehiclesView();
        UpdateVehicleCombat();

        UpdateRocketLandingCombat();
        FilterElapsedRockets();
        UpdateRocketLauncherOrientation();
        OperateRocketLaunchers();
        UpdateRocketLauncherCombatState();
        UpdateRocketLauncherView();

        UpdateProjectileHits();
        FilterDeadProjectiles();
        UpdateTurelsOrientation();
        OperateTurels();
        UpdateTurelsCombatState();
        UpdateTurelView();
    }

    private void ReadDriveVehicleInput() {
        const float maxSteerAngle = 35;
        var gasInput = Input.GetAxis("Vertical");
        var steerInput = Input.GetAxis("Horizontal");

        vehicleService.SetVehicleGasThrottle(vehicleIndex: 0, gasInput);
        vehicleService.SetVehicleSteer(vehicleIndex: 0, steerInput * maxSteerAngle);
    }

    private void UpdateVehicleCombat() {
        combatService.UpdateAgentPosition(driveVehicleCombatId, driveVehicle.BodyPose.position);
        combatService.ApplyExplosionDamage(driveVehicleCombatId, driveVehicle.BodyPose.position, radius: 1, damage: 0);
    }

    private void SpawnDriveVehicle(Vector3 driveVehiclePosition) {
        driveVehicle = new Vehicle(driveVehicleBlueprint.physicsData);
        vehicleService.CreateVehicle(driveVehiclePosition, driveVehicleBlueprint.physicsData);
        vehicleView.AddVehicle(driveVehiclePosition, driveVehicleBlueprint.physicsData, driveVehicleBlueprint.visualsId);
        
        vehicles.Add(driveVehicle);
        driveVehicleCombatId = combatService.RegisterAgent(driveVehiclePosition, turelsCombatGroupId);
    }

    private void SpawnTrailerVehicle(Vector3 position) {
        var trailerVehicle = new Vehicle(trailerVehicleBlueprint.physicsData);
        vehicleService.CreateVehicle(position, trailerVehicleBlueprint.physicsData);
        vehicleView.AddVehicle(position, trailerVehicleBlueprint.physicsData, trailerVehicleBlueprint.visualsId);

        vehicles.Add(trailerVehicle);
        var lastIndex = vehicles.Count - 1;
        vehicleService.MakeTowingConnection(
            headVehicleIndex: lastIndex - 1, 
            tailVehicleIndex: lastIndex);
    }

    private void ReadVehiclesOrientation() {
        for (int vehicleIndex = 0; vehicleIndex < vehicles.Count; vehicleIndex++) {
            var vehicle = vehicles[vehicleIndex];
            var vehiclePhysicsRigIndex = vehicleIndex;
            
            var vehiclePose = vehicleService.GetVehiclePose(vehiclePhysicsRigIndex);
            vehicle.OrientBody(vehiclePose);

            for (int wheelAxisIndex = 0; wheelAxisIndex < vehicle.WheelAxisPoses.Length; wheelAxisIndex++) {
                var wheelAxisPose = vehicleService.GetVehicleWheelAxisPose(vehiclePhysicsRigIndex, wheelAxisIndex);
                vehicle.OrientWheelAxis(wheelAxisIndex, wheelAxisPose);
            }   

            if (vehicle.TowingTonqueRotation.HasValue) {
                var towingTonguePose = vehicleService.GetTowingTonguePose(vehiclePhysicsRigIndex);
                vehicle.OrientTowingTonque(towingTonguePose);
            }
        }
    }

    private void UpdateVehiclesView() {
        for (int vehicleIndex = 0; vehicleIndex < vehicles.Count; vehicleIndex++) {
            var vehicle = vehicles[vehicleIndex];
            var vehicleViewIndex = vehicleIndex;
            vehicleView.UpdateVehiclePose(vehicleViewIndex, vehicle.BodyPose);

            for (int wheelAxisIndex = 0; wheelAxisIndex < vehicle.WheelAxisPoses.Length; wheelAxisIndex++) {
                vehicleView.UpdateWheelAxisPose(vehicleViewIndex, wheelAxisIndex, vehicle.WheelAxisPoses[wheelAxisIndex]);
            }   

            if (vehicle.TowingTonqueRotation.HasValue) {
                vehicleView.UpdateTowingTonguePose(vehicleViewIndex, vehicle.TowingTonqueRotation.Value);
            }
        }
    }

    private void SpawnRocketLauncher(Vehicle host, RocketLauncherConfig launcherConfig) {
        var launcherId = rocketLauncherIdCounter++;
        var rocketLauncher = new RocketLauncher(launcherId, host.BodyPose.position, launcherConfig);
        rocketLaunchers.Add(rocketLauncher);

        rocketLauncherToVehicle[rocketLauncher.Id] = host;

        var rocketLauncherCombatId = combatService.RegisterAgent(rocketLauncher.Position, turelsCombatGroupId);
        rocketLauncherToCombatId[launcherId] = rocketLauncherCombatId;

        rocketLauncherRocketsRegistry[launcherId] = new List<Rocket>();
        
        view.AddRocketLauncher(launcherId, rocketLauncher.Position);
    }

    private void UpdateRocketLauncherOrientation() {
        foreach (var rocketLauncher in rocketLaunchers) {
            var rocketLauncherHost = rocketLauncherToVehicle[rocketLauncher.Id];
            rocketLauncher.Translate(rocketLauncherHost.BodyPose.position); 
        }
    }

    private void OperateRocketLaunchers() {
        foreach (var rocketLauncher in rocketLaunchers) {
            var launcherCombatId = rocketLauncherToCombatId[rocketLauncher.Id];
            if (combatService.GetClosestEnemyAgentInRange(launcherCombatId, rocketLauncher.Radius, out var agentInfo, excludeGroup: turelsCombatGroupId)) {
                rocketLauncher.Aim(agentInfo.position);
            }
            
            if (rocketLauncher.Launch(Time.time, out var trajectory)) {
                SpawnRocket(rocketLauncher, trajectory);
            }
        }
    }

    private void UpdateRocketLauncherCombatState() {
        foreach (var rocketLauncher in rocketLaunchers) {
            var rocketLauncherCombatId = rocketLauncherToCombatId[rocketLauncher.Id];
            combatService.UpdateAgentPosition(rocketLauncherCombatId, rocketLauncher.Position);
        }
    }

    private void SpawnRocket(RocketLauncher rocketLauncher, RocketTrajectory trajectory) {
        var nextRocketId = rocketIdCounter++;
        var rocket = new Rocket(nextRocketId, trajectory, Time.time, rocketLauncher.RocketFlyDuration);
        rocketLauncherRocketsRegistry[rocketLauncher.Id].Add(rocket);
        view.ShowRocketFly(rocketLauncher.Id, nextRocketId, trajectory, rocketLauncher.RocketFlyDuration);
    }

    private void UpdateRocketLandingCombat() {
        foreach (var rocketLauncher in rocketLaunchers) {
            var launcherCombatId = rocketLauncherToCombatId[rocketLauncher.Id];
            
            foreach (var rocket in rocketLauncherRocketsRegistry[rocketLauncher.Id]) {
                if (rocket.ForwardLandingTime(Time.time)) {
                    combatService.ApplyExplosionDamage(launcherCombatId, rocket.Trajectory.landPoint, 3, rocketLauncher.RocketDamage);
                    var center = rocket.Trajectory.landPoint;
                    var color = Color.red;
                    var duration = 1f;
                    var radius = 3;
                    Debug.DrawLine(center, center + Vector3.right * radius, color, duration);
                    Debug.DrawLine(center, center + Vector3.left * radius, color, duration);
                    Debug.DrawLine(center, center + Vector3.forward * radius, color, duration);
                    Debug.DrawLine(center, center + Vector3.back * radius, color, duration);
                    view.ShowRocketExplosion(rocketLauncher.Id, rocket.Id);
                }
            }   
        }
    }

    private void FilterElapsedRockets() {
        foreach (var rocketLauncher in rocketLaunchers) {
            var launcherRockets = rocketLauncherRocketsRegistry[rocketLauncher.Id];
            for (int i = 0; i < launcherRockets.Count; i++) {
                var rocket = launcherRockets[i];
                if (rocket.Landed) {
                    launcherRockets.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    private void UpdateRocketLauncherView() {
        foreach (var rocketLauncher in rocketLaunchers) {
            view.UpdateRocketLauncherOrientation(rocketLauncher.Id, rocketLauncher.Position, rocketLauncher.AimPoint, rocketLauncher.RocketAmplitude);
        }
    }

    private void SpawnTurel(Vehicle host, TurelConfig turelConfig) {
        var turelId = turelIdCounter++;
        var turel = new Turel(turelId, host.BodyPose.position, turelConfig);
        turels.Add(turel);

        turelToVehicle[turel.Id] = host;
        
        var turelCombatId = combatService.RegisterAgent(turel.Position, groupId: turelsCombatGroupId);
        turelToCombatId[turel.Id] = turelCombatId;

        var turelProjectilesGroupId = projectileService.AddGroup();
        turelToProjectileGroupId[turel.Id] = turelProjectilesGroupId;
        
        view.AddTurel(turelId, turel.Position);
    }

    private void UpdateTurelsOrientation() {
        foreach (var turel in turels) {
            var turelHost = turelToVehicle[turel.Id];
            turel.Move(turelHost.BodyPose.position);
        }
    }

    private void OperateTurels() {
        foreach (var turel in turels) {    
            var turelCombatId = turelToCombatId[turel.Id];
            
            if (combatService.GetClosestEnemyAgentInRange(turelCombatId, 20, out var closestEnemyAgent, excludeGroup: turelsCombatGroupId)) {
                var aimPoint = closestEnemyAgent.position + 0.5f * closestEnemyAgent.height * Vector3.up;
                turel.Aim(Time.deltaTime, aimPoint);
            }

            if (turel.Fire(Time.time, out var bullet)) {
                SpawnBulletProjectile(turel, bullet);
            }
        }
    }

    private void UpdateTurelsCombatState() {
        foreach (var turel in turels) {    
            var turelCombatId = turelToCombatId[turel.Id];
            combatService.UpdateAgentPosition(turelCombatId, turel.Position);
        }
    }

    private void UpdateTurelView() {
        foreach (var turel in turels) {
            view.UpdateTurelOrientation(turel.Id, turel.Position, turel.GunForward);
        }
    }

    private void SpawnBulletProjectile(Turel turel, Bullet bullet) {
        var projectileGroupId = turelToProjectileGroupId[turel.Id];
        var projectileId = projectileService.CreateProjectile(projectileGroupId, bullet.firePoint, bullet.velocity, 5f);
        view.ShowBulletShoot(turel.Id, projectileId, bullet.velocity);
    }

    private void FilterDeadProjectiles() {
        foreach (var turel in turels) {
            var projectileGroup = turelToProjectileGroupId[turel.Id];
            projectilesStateBuffer.Clear();
            projectileService.GetGroupProjectiles(projectileGroup, projectilesStateBuffer);
            
            foreach (var projectileState in projectilesStateBuffer) {
                if (projectileState.isAged) {
                    view.ShowBulletDisappear(turel.Id, projectileState.id);
                }
            }
        }
    }

    private void UpdateProjectileHits() {   
        foreach (var turel in turels) {
            var projectileGroup = turelToProjectileGroupId[turel.Id];
            projectilesStateBuffer.Clear();
            projectileService.GetGroupProjectiles(projectileGroup, projectilesStateBuffer);

            var combatId = turelToCombatId[turel.Id];
            foreach (var projectileState in projectilesStateBuffer) {
                if (projectileState.isAged)
                    continue;

                if (combatService.ApplyProjectileDamage(combatId, projectileState.position, projectileState.velocity, turel.BulletDamage)) {
                    projectileService.KillProjectile(projectileState.id);
                    view.ShowBulletCrash(turel.Id, projectileState.id);
                }
            }
        }
    }
}
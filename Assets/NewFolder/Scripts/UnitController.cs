using UnityEngine;
using System.Collections.Generic;
using Unity.Profiling;
using System.Linq;
using UnityEngine.Assertions;

public class UnitController {

    private readonly UnitView unitView;
    private readonly LocalAvoidanceService localAvoidanceService;
    private readonly NavigationService navigationService;
    private readonly CombatService combatService;
    private readonly PhysicsService physicsService;
    private readonly RewardsMediator rewardsMediator;
    private readonly ProjectileService projectileService;

    private readonly GameObject pointsRewardVisualsPrefab;

    private readonly VehicleService vehicleService;
    private readonly UnitVehicleData foeVehicleData;
    private readonly SoundManager soundManager;

    private float lastTimeProduced = float.MinValue;
    private Transform[] spawnPoints;
    private Transform targetPoint;
    private int unitsCombatGroup;
    private int unitsCount;
    private int idCounter;

    private readonly List<Unit> units = new List<Unit>();
    private readonly Dictionary<int, int> unitIdToAvoidanceId = new Dictionary<int, int>();
    private readonly Dictionary<int, int> unitIdToCombatId = new Dictionary<int, int>();
    private readonly Dictionary<int, int> unitIdToPhysicsId = new Dictionary<int, int>();

    private float lastTimeProducedVehicle;
    private Transform[] vehiclesSpawnPoints;
    private int maxVehiclesCount;
    private int lastSpawnIndex;

    private readonly List<UnitVehicle> vehicles = new ();
    private readonly List<int> vehiclePhysics = new ();
    private readonly List<int> vehicleCombats = new();
    private readonly List<int> vehicleSoundLoop = new();
    private readonly List<int> vehicleViewIds = new();
    private readonly List<Turel> vehicleTurels = new ();
    private readonly List<int> vehicleTurelProjectileGroupIds = new ();
    private readonly List<ProjectileState> projectilesStateBuffer = new (64);
    private int rocketIdCounter;
    private readonly List<RocketLauncher> vehicleRocketLaunchers = new ();
    private readonly List<List<Rocket>> vehicleRocketsRegistry = new ();

    public UnitController(LocalAvoidanceService localAvoidanceService, NavigationService navigationService, UnitView crowdView,
        Transform[] spawnPoints, Transform targetPoint, int unitsCount, CombatService combatService, PhysicsService physicsService,
        RewardsMediator rewardsMediator, VehicleService vehicleService, UnitVehicleData foeVehicle, int maxVehiclesCount,
        SoundManager soundManager, ProjectileService projectileService, GameObject pointsRewardVisualsPrefab) {
        this.localAvoidanceService = localAvoidanceService;
        this.navigationService = navigationService;
        this.unitView = crowdView;
        this.spawnPoints = spawnPoints;
        this.targetPoint = targetPoint;
        this.unitsCount = unitsCount;
        this.combatService = combatService;
        this.physicsService = physicsService;
        this.rewardsMediator = rewardsMediator;

        this.vehicleService = vehicleService;
        this.foeVehicleData = foeVehicle;
        this.vehiclesSpawnPoints = spawnPoints; // reusing
        this.maxVehiclesCount = maxVehiclesCount;
        this.soundManager = soundManager;
        this.projectileService = projectileService;
        this.pointsRewardVisualsPrefab = pointsRewardVisualsPrefab;
    }

    public void Init() {
        navigationService.SetGoal(targetPoint.position);
    }

    private void ProduceNewUnits() {
        if (units.Count > unitsCount) 
            return;

        if (lastTimeProduced + 0.1f > Time.time)
            return;
        
        lastTimeProduced = Time.time;
        foreach (var spawnPoint in spawnPoints) {    
            SpawnUnit(spawnPoint.position);
        }
    }

    private void ProduceNewVehicleUnits() {
        if (vehicles.Count > maxVehiclesCount)
            return;

        if (lastTimeProducedVehicle + 1f > Time.time)
            return;

        lastTimeProducedVehicle = Time.time;
        var nextSpawnIndex = lastSpawnIndex++ % vehiclesSpawnPoints.Length;
        SpawnVehicle(vehiclesSpawnPoints[nextSpawnIndex].position);
    }

    private void SpawnUnit(Vector3 position) {
        Unit newUnit = new Unit(idCounter++, position, Quaternion.identity, 10f);
        units.Add(newUnit);
        
        var agentId = localAvoidanceService.AddAgent(position);
        unitIdToAvoidanceId[newUnit.Id] = agentId;
        
        var combatAgentId = combatService.RegisterAgent(position, alie: false);
        unitIdToCombatId[newUnit.Id] = combatAgentId;

        var physicsAgentId = physicsService.RegisterPhysicsEntity(position, .5f, 0.15f);
        unitIdToPhysicsId[newUnit.Id] = physicsAgentId;
        
        unitView.AddUnit(newUnit.Id, position);
    }

    private static readonly ProfilerMarker readUnitOrientationMarker = new ProfilerMarker("ReadOrientation");
    private static readonly ProfilerMarker filterDeadUnitsMarker = new ProfilerMarker("FilterDeadUnits");
    private static readonly ProfilerMarker readCombatServiceInputMarker = new ProfilerMarker("ReadCombatServiceInput");
    private static readonly ProfilerMarker setCombatStateMarker = new ProfilerMarker("SetCombatState");
    private static readonly ProfilerMarker updateUnitsOrientationMarker = new ProfilerMarker("UpdateUnitsOrientation");
    private static readonly ProfilerMarker updateViewPoseMarker = new ProfilerMarker("UpdateViewPose");

    public void Update() {
        UpdateGoal();

        using (readUnitOrientationMarker.Auto())
            ReadUnitOrientation();
        using (filterDeadUnitsMarker.Auto())
            FilterRemovedUnits();
        
        ProduceNewUnits();
        OperateUnits();

        using (readCombatServiceInputMarker.Auto())
            ReadCombatServiceInput();
        using (setCombatStateMarker.Auto())
            SetCombatStateFromUnit();
        using (updateUnitsOrientationMarker.Auto())
            UpdateUnitsOrientation();
        using (updateViewPoseMarker.Auto())
            UpdateViewPose();

        ProduceNewVehicleUnits();
        ReadVehiclesOrientation();
        ReadVehiclesCombat();
        FilterDeadVehicles();

        UpdateVehicleNavigation();
        UpdateVehicleCombat();
        UpdateVehiclePhysics();
        UpdateVehiclesSounds();
        UpdateVehiclesView();

        OperateWeapons();
        FilterDeadProjectiles();
        UpdateProjectileHits();
        UpdateRocketLandingCombat();
        FilterElapsedRockets();
    }

    private void UpdateGoal() {
        var tractor = vehicleService.GetVehicleState(0);
        targetPoint.position = tractor.position;
        
        navigationService.SetGoal(tractor.position);
    }

    private void OperateUnits() {
        foreach (var unit in units) {
            if (!unit.Grouned || !unit.IsAlive)
                continue;
            
            var unitCombatId = unitIdToCombatId[unit.Id];
            if (!combatService.GetClosestEnemyAgentInRange(unitCombatId, 2, out var closestFoe))
                continue;
                
            if (unit.TryDirectFrontAttack(Time.time, out var damage)) {
                combatService.ApplyDirectDamage(unitCombatId, closestFoe.id, damage);
                unitView.ShowDirectFrontAttack(unit.Id);
            }
        }
    }

    private void ReadUnitOrientation() {
        foreach (var unit in units) {
            var physicsAgentId = unitIdToPhysicsId[unit.Id];
            var avoidanceAgentId = unitIdToAvoidanceId[unit.Id];
            
            var physicsPose = physicsService.GetEntityPose(physicsAgentId);
            var keepFlying = !unit.Grouned && physicsPose.InMotion;
            var becomeGrounded = !unit.Grouned && !physicsPose.InMotion;
            var keepsGrouned = unit.Grouned && !physicsPose.InMotion;
            
            if (keepFlying) {
                unit.Fly(physicsPose.Position, physicsPose.Rotation);
            } else if (becomeGrounded) {
                unit.Stand(physicsService.GetGroundPosition(unit.Position));
                physicsService.SetPhysicsActive(physicsAgentId, false);
            } else if (keepsGrouned && unit.IsAlive) {
                localAvoidanceService.GetAgentPositionAndRotation(avoidanceAgentId, out var pos, out var rot);
                unit.Move(pos, rot);
            }
        }
    }

    private void UpdateUnitsOrientation() {
        foreach (var unit in units) {
            if (unit.Grouned) {
                var avoidanceAgentId = unitIdToAvoidanceId[unit.Id];
                localAvoidanceService.SetAgentPosition(avoidanceAgentId, unit.Position);
                var flowVector = navigationService.GetFlowVector(unit.Position);
                localAvoidanceService.SetPreferedVelocity(avoidanceAgentId, flowVector);
            }
        }
    }

    private void ReadCombatServiceInput() {
        foreach (var unit in units) {
            if (!unit.IsAlive)
                continue;
                
            var combatId = unitIdToCombatId[unit.Id];
            var combatAgentState = combatService.GetAgentState(combatId);
            
            bool anyDamage = false;
            if (combatAgentState.exploded && unit.TryTakeExplosionHit(Time.time, combatAgentState.damage, combatAgentState.damageSourcePosition)) {
                var unitPhysicsId = unitIdToPhysicsId[unit.Id];
                physicsService.UpdatePhysicsEntityPosition(unitPhysicsId, unit.Position);
                physicsService.SetPhysicsActive(unitPhysicsId, true);
                physicsService.AddExplosionForce(unitPhysicsId, 10, combatAgentState.damageSourcePosition, 4f, 1, ForceMode.Impulse);
                // unitView.ShowTakeExplosionHit(unit.Id);
                anyDamage = true;
            }

            if (combatAgentState.projectiled) {
                unit.TakeProjectileHit(combatAgentState.damage, combatAgentState.damageSourcePosition);
                // unitView.ShowTakeProjectileHit(unit.Id, combatAgentState.damageSourcePosition);
                anyDamage = true;
            }

            combatService.ClearAgentState(combatId);

            if (anyDamage) {
                unitView.ShowTakeHit(unit.Id);
            }

            if (!unit.IsAlive) {
                if (unit.DeathCause.type == Unit.DamageType.Projectile) {
                    unitView.ShowDeathByProjectile(unit.Id, unit.DeathCause.damageSource, blownAway: unit.Grouned);
                } else {
                    unitView.ShowDisolveDeath(unit.Id);
                }
                
                combatService.UnregisterAgent(combatId);
                unitIdToCombatId.Remove(unit.Id);

                rewardsMediator.AddReward(unit.Position, radius: 1, RewardType.Points, pointsRewardVisualsPrefab, new RewardConfigs {});
            }
        }
    }

    private void SetCombatStateFromUnit() {
        foreach (var unit in units) {
            if (!unit.IsAlive)
                continue;

            var combatId = unitIdToCombatId[unit.Id];
            combatService.UpdateAgentPosition(combatId, unit.Position);
        }
    }

    private void FilterRemovedUnits() {
        for (int i = 0; i < units.Count; i++) {
            var unit = units[i];
            if (unit.ToBeRemoved) {
                DespawnUnitAt(i);
                i--;
            }
        }
    }

    private void DespawnUnitAt(int unitIndex) {
        var unitId = units[unitIndex].Id;
        units.RemoveAt(unitIndex);
        localAvoidanceService.RemoveAgent(unitIdToAvoidanceId[unitId]);
        unitIdToAvoidanceId.Remove(unitId);
        // combatService.UnregisterAgent(unitIdToCombatId[unitId]);
        // unitIdToCombatId.Remove(unitId);
        physicsService.UnregisterPhysicsEntity(unitIdToPhysicsId[unitId]);
        unitIdToPhysicsId.Remove(unitId);
        unitView.RemoveUnit(unitId);
    }

    private void UpdateViewPose() {
        foreach (var unit in units) {
            unitView.UpdateUnitPositionAndRotation(unit.Id, unit.Position, unit.Rotation);
        }
    }

    private void SpawnVehicle(Vector3 position) {
        var vehicle = new UnitVehicle(foeVehicleData);
        vehicles.Add(vehicle);
        
        var viewId = unitView.AddUnitVehicle(position, vehicle.VisualsData.baseGeometry, vehicle.VisualsData.wheelGeometry);
        vehicleViewIds.Add(viewId);

        var combatAgentId = combatService.RegisterAgent(position, alie: false);
        vehicleCombats.Add(combatAgentId);

        var physicsAgentId = vehicleService.CreateVehicle(position, vehicle.PhysicsPrefab);
        vehiclePhysics.Add(physicsAgentId);

        var soundLoopId = soundManager.StartLoop(position, vehicle.EngineIdleSound);
        vehicleSoundLoop.Add(soundLoopId);

        if (vehicle.WeaponsData.rocketLauncherConfig != null) {
            unitView.SetRocketLauncherWeapon(viewId, vehicle.Position, vehicle.WeaponsData.rocketLauncherConfig.visualsPrefab);

            var vehicleRocketLauncher = new RocketLauncher(-1, vehicle.Position, vehicle.WeaponsData.rocketLauncherConfig);
            vehicleRocketLaunchers.Add(vehicleRocketLauncher);

            var rocketRegistry = new List<Rocket>();
            vehicleRocketsRegistry.Add(rocketRegistry);
        } else {
            vehicleRocketLaunchers.Add(null);
            vehicleRocketsRegistry.Add(null);
        }

        if (vehicle.WeaponsData.turelConfig != null) {
            Assert.IsNull(vehicleRocketLaunchers[vehicles.Count - 1]);
            unitView.SetTurelWeapon(viewId, vehicle.Position, vehicle.WeaponsData.turelConfig.visualsPrefab);

            var turel = new Turel(-1, vehicle.Position, vehicle.WeaponsData.turelConfig);
            vehicleTurels.Add(turel);

            var turelProjectileGroupId = projectileService.AddGroup();
            vehicleTurelProjectileGroupIds.Add(turelProjectileGroupId);
        } else {
            vehicleTurels.Add(null);
            vehicleTurelProjectileGroupIds.Add(-1);
        }
    }

    private void DespawnVehicleAt(int vehicleIndex) {
        vehicles.RemoveAt(vehicleIndex);

        unitView.RemoveVehicleView(vehicleViewIds[vehicleIndex]);
        vehicleViewIds.RemoveAt(vehicleIndex);

        combatService.UnregisterAgent(vehicleCombats[vehicleIndex]);
        vehicleCombats.RemoveAt(vehicleIndex);

        vehicleService.DeleteVehicle(vehiclePhysics[vehicleIndex]);
        vehiclePhysics.RemoveAt(vehicleIndex);

        soundManager.StopLoop(vehicleSoundLoop[vehicleIndex]);
        vehicleSoundLoop.RemoveAt(vehicleIndex);

        vehicleRocketLaunchers.RemoveAt(vehicleIndex);
        vehicleRocketsRegistry.RemoveAt(vehicleIndex);

        if (vehicleTurels[vehicleIndex] != null) {
            var projectileGroupId = vehicleTurelProjectileGroupIds[vehicleIndex];
            projectileService.RemoveGroup(projectileGroupId);
        }
        vehicleTurels.RemoveAt(vehicleIndex);
        vehicleTurelProjectileGroupIds.RemoveAt(vehicleIndex);
    }

    private void ReadVehiclesOrientation() {
        for (int vehicleIndex = 0; vehicleIndex < vehicles.Count; vehicleIndex++) {
            var vehicle = vehicles[vehicleIndex];
            var physicsId = vehiclePhysics[vehicleIndex];
            
            var vehiclePose = vehicleService.GetVehicleState(physicsId);
            vehicle.UpdatePhysicsState(vehiclePose);

            var vehicleTurel = vehicleTurels[vehicleIndex];
            vehicleTurel?.Move(vehicle.Position);
            
            var vehicleRocketLauncher = vehicleRocketLaunchers[vehicleIndex];
            vehicleRocketLauncher?.Translate(vehicle.Position);
        }
    }

    private void ReadVehiclesCombat() {
        for (int i = 0; i < vehicles.Count; i++) {
            var vehicle = vehicles[i];
            var vehicleCombatId = vehicleCombats[i];

            var vehicleCombatState = combatService.GetAgentState(vehicleCombatId);
            if (vehicleCombatState.projectiled || vehicleCombatState.exploded) {
                vehicle.TakeDamage(vehicleCombatState.damage);
            }

            combatService.ClearAgentState(vehicleCombatId);

            if (!vehicle.IsAlive) {
                rewardsMediator.AddReward(vehicle.Position, 3, RewardType.TurelWeapon, vehicle.WeaponsData.turelConfig.visualsPrefab.gameObject, new RewardConfigs {
                    turelConfig = vehicle.WeaponsData.turelConfig
                });
            }
        }
    }

    private void FilterDeadVehicles() {
        for (int i = 0; i < vehicles.Count; i++) {
            var vehicle = vehicles[i];
            if (!vehicle.IsAlive) {
                DespawnVehicleAt(i);
                i--;
            }
        }
    }

    private void UpdateVehicleNavigation() {
        foreach (var vehicle in vehicles) {
            var distance = Vector3.Distance(targetPoint.position, vehicle.Position);

            var gasDistance = 10;
            var gas = Mathf.Floor(Mathf.Clamp(distance, 0, gasDistance) / gasDistance);
            vehicle.Throttle(gas, Time.deltaTime, false);
            
            var stopDistance = 5f;
            var breaks = 1 - Mathf.Floor(Mathf.Clamp(distance, 0, stopDistance) / stopDistance);
            vehicle.Breaks(breaks);
            var flowVector = navigationService.GetFlowVector(vehicle.Position);
            vehicle.SteerToward(flowVector);
        }
    }

    private void OperateWeapons() {
        for (int vehicleIndex = 0; vehicleIndex < vehicles.Count; vehicleIndex++) {
            var vehicel = vehicles[vehicleIndex];
            var vehicleCombatId = vehicleCombats[vehicleIndex];

            var turel = vehicleTurels[vehicleIndex];
            var rocketLauncher = vehicleRocketLaunchers[vehicleIndex];
            
            if (combatService.GetClosestEnemyAgentInRange(vehicleCombatId, 20, out var agentInfo)) {
                if (turel != null) {
                    Assert.IsNull(rocketLauncher);

                    turel.Aim(Time.deltaTime, agentInfo.position + 0.5f * agentInfo.height * Vector3.up);
                    if (turel.Fire(Time.time, out var bullet)) {
                        SpawnBulletProjectile(vehicleIndex, turel, bullet);
                    }
                }

                if (rocketLauncher != null) {
                    Assert.IsNull(turel);

                    rocketLauncher.Aim(agentInfo.position);
                    if (rocketLauncher.Launch(Time.time, out var rocketTrajectory)) {
                        SpawnRocket(vehicleIndex, rocketLauncher, rocketTrajectory);
                    }
                }
            }
        }
    }

    private void UpdateVehicleCombat() {
        for (int vehicleIndex = 0; vehicleIndex < vehicles.Count; vehicleIndex++) {
            var vehicle = vehicles[vehicleIndex];
            var vehicelCombatId = vehicleCombats[vehicleIndex];
            combatService.UpdateAgentPosition(vehicelCombatId, vehicle.Position);
        }
    }

    private void UpdateVehiclePhysics() {
        for (int vehicleIndex = 0; vehicleIndex < vehicles.Count; vehicleIndex++) {
            var vehicle = vehicles[vehicleIndex];
            var vehiclePhysicId = vehiclePhysics[vehicleIndex];
            vehicleService.SetVehicleEngineTorque(vehiclePhysicId, vehicle.MotorTorque);
            vehicleService.SetVehicleSteer(vehiclePhysicId, vehicle.SteerDegrees);
            vehicleService.SetVehicleBreaks(vehiclePhysicId, vehicle.BreaksTorque);
        }
    }

    private void UpdateVehiclesSounds() {
        for (int vehicleIndex = 0; vehicleIndex < vehicles.Count; vehicleIndex++) {
            var vehicle = vehicles[vehicleIndex];
            var vehicleLoopId = vehicleSoundLoop[vehicleIndex];
            soundManager.UpdateLoop(vehicleLoopId, vehicle.Position, vehicle.DrivePower, vehicle.DrivePower);
        }
    }

    private void UpdateVehiclesView() {
        for (int vehicleIndex = 0; vehicleIndex < vehicles.Count; vehicleIndex++) {
            var vehicle = vehicles[vehicleIndex];
            var vehicleViewIndex = vehicleViewIds[vehicleIndex];
            unitView.UpdateVehiclePose(vehicleViewIndex, vehicle.PhysicsState);

            var vehicleViewId = vehicleViewIds[vehicleIndex];
            var turel = vehicleTurels[vehicleIndex];
            if (turel != null) {
                unitView.UpdateTurelOrientation(vehicleViewId, turel.Position, turel.GunForward);
            }
            var rocketLauncher = vehicleRocketLaunchers[vehicleIndex];
            if (rocketLauncher != null) {
                unitView.UpdateRocketLauncherOrientation(vehicleViewId, rocketLauncher.Position, rocketLauncher.AimPoint, rocketLauncher.RocketAmplitude);
            }
        }
    }

    private void SpawnBulletProjectile(int vehicleIndex, Turel turel, Bullet bullet) {
        var projectileGroupId = vehicleTurelProjectileGroupIds[vehicleIndex];
        var projectileId = projectileService.CreateProjectile(projectileGroupId, bullet.firePoint, bullet.velocity, 5f);
        var vehicleViewId = vehicleViewIds[vehicleIndex];
        unitView.ShowBulletShoot(vehicleViewId, projectileId, bullet.velocity);
        soundManager.PlayEffect(bullet.firePoint, turel.BulletShootAudioClips);
    }

    private void FilterDeadProjectiles() {
        for (int i = 0; i < vehicles.Count; i++) {      
            if (vehicleTurels[i] == null)
                continue;

            var projectileGroup = vehicleTurelProjectileGroupIds[i];
            projectilesStateBuffer.Clear();
            projectileService.GetGroupProjectiles(projectileGroup, projectilesStateBuffer);
            
            foreach (var projectileState in projectilesStateBuffer) {
                if (projectileState.isAged) {
                    unitView.ShowBulletDisappear(i, projectileState.id);
                }
            }
        }
    }

    private void UpdateProjectileHits() {   
        for (int vehicleIndex = 0; vehicleIndex < vehicles.Count; vehicleIndex++) {      
            var turel = vehicleTurels[vehicleIndex];
            if (turel == null)
                continue;

            var projectileGroup = vehicleTurelProjectileGroupIds[vehicleIndex];
            projectilesStateBuffer.Clear();
            projectileService.GetGroupProjectiles(projectileGroup, projectilesStateBuffer);

            var combatId = vehicleCombats[vehicleIndex];
            foreach (var projectileState in projectilesStateBuffer) {
                if (projectileState.isAged)
                    continue;

                if (combatService.ApplyProjectileDamage(combatId, projectileState.position, projectileState.velocity, turel.BulletDamage)) {
                    projectileService.KillProjectile(projectileState.id);
                    var vehicleViewId = vehicleViewIds[vehicleIndex];
                    unitView.ShowBulletCrash(vehicleViewId, projectileState.id);
                }
            }
        }
    }

    private void SpawnRocket(int vehicleIndex, RocketLauncher rocketLauncher, RocketTrajectory trajectory) {
        var nextRocketId = rocketIdCounter++;
        var rocket = new Rocket(nextRocketId, trajectory, Time.time, rocketLauncher.RocketFlyDuration);
        var rocketsRegistry = vehicleRocketsRegistry[vehicleIndex];
        rocketsRegistry.Add(rocket);
        unitView.ShowRocketFly(vehicleIndex, nextRocketId, trajectory, rocketLauncher.RocketFlyDuration);
        soundManager.PlayEffect(trajectory.launchPoint, rocketLauncher.RocketLaunchEffects);
    }

    private void UpdateRocketLandingCombat() {
        for (int vehicleIndex = 0; vehicleIndex < vehicles.Count; vehicleIndex++) {      
            var rocketLauncher = vehicleRocketLaunchers[vehicleIndex];
            if (rocketLauncher == null)
                continue;
            
            var vehicleViewId = vehicleViewIds[vehicleIndex];
            var vehicleCombatId = vehicleCombats[vehicleIndex];
            var rocketLauncherRockets = vehicleRocketsRegistry[vehicleIndex];
            foreach (var rocket in rocketLauncherRockets) {
                if (rocket.ForwardLandingTime(Time.time)) {
                    combatService.ApplyExplosionDamage(vehicleCombatId, rocket.Trajectory.landPoint, 3, rocketLauncher.RocketDamage);
                    unitView.ShowRocketExplosion(vehicleViewId, rocket.Id);
                    soundManager.PlayEffect(rocket.Trajectory.landPoint, rocketLauncher.ExplodeEffectClips);
                }
            }   
        }
    }

    private void FilterElapsedRockets() {
        for (int vehicleIndex = 0; vehicleIndex < vehicles.Count; vehicleIndex++) {      
            var rocketLauncher = vehicleRocketLaunchers[vehicleIndex];
            if (rocketLauncher == null)
                continue;
            
            var rocketLauncherRockets = vehicleRocketsRegistry[vehicleIndex];
            for (int i = 0; i < rocketLauncherRockets.Count; i++) {
                var rocket = rocketLauncherRockets[i];
                if (rocket.Landed) {
                    rocketLauncherRockets.RemoveAt(i);
                    i--;
                }
            }
        }
    }

}
using UnityEngine;
using System.Collections.Generic;
using Unity.Profiling;
using System.Linq;
using UnityEngine.Assertions;

public class EnemyController {

    private readonly WeaponController weaponController;

    private readonly EnemyView enemyView;
    private readonly LocalAvoidanceService localAvoidanceService;
    private readonly NavigationService navigationService;
    private readonly CombatService combatService;
    private readonly PhysicsService physicsService;
    private readonly RewardsMediator rewardsMediator;

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
    private readonly List<int> vehicleWeaponIds = new();

    public EnemyController(LocalAvoidanceService localAvoidanceService, NavigationService navigationService, EnemyView crowdView,
        Transform[] spawnPoints, Transform targetPoint, int unitsCount, CombatService combatService, PhysicsService physicsService,
        RewardsMediator rewardsMediator, VehicleService vehicleService, UnitVehicleData foeVehicle, int maxVehiclesCount,
        SoundManager soundManager, GameObject pointsRewardVisualsPrefab, WeaponController weaponController) {
        this.localAvoidanceService = localAvoidanceService;
        this.navigationService = navigationService;
        this.enemyView = crowdView;
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
        this.pointsRewardVisualsPrefab = pointsRewardVisualsPrefab;

        this.weaponController = weaponController;
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
        
        enemyView.AddUnit(newUnit.Id, position);
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
                enemyView.ShowDirectFrontAttack(unit.Id);
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
                enemyView.ShowTakeHit(unit.Id);
            }

            if (!unit.IsAlive) {
                if (unit.DeathCause.type == Unit.DamageType.Projectile) {
                    enemyView.ShowDeathByProjectile(unit.Id, unit.DeathCause.damageSource, blownAway: unit.Grouned);
                } else {
                    enemyView.ShowDisolveDeath(unit.Id);
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
        enemyView.RemoveUnit(unitId);
    }

    private void UpdateViewPose() {
        foreach (var unit in units) {
            enemyView.UpdateUnitPositionAndRotation(unit.Id, unit.Position, unit.Rotation);
        }
    }

    private void SpawnVehicle(Vector3 position) {
        var vehicle = new UnitVehicle(foeVehicleData);
        vehicles.Add(vehicle);
        
        var viewId = enemyView.AddUnitVehicle(position, vehicle.VisualsPrefab);
        vehicleViewIds.Add(viewId);

        var combatAgentId = combatService.RegisterAgent(position, alie: false);
        vehicleCombats.Add(combatAgentId);

        var physicsAgentId = vehicleService.CreateVehicle(position, vehicle.PhysicsPrefab);
        vehiclePhysics.Add(physicsAgentId);

        var soundLoopId = soundManager.StartLoop(position, vehicle.EngineIdleSound);
        vehicleSoundLoop.Add(soundLoopId);

        var weaponId = weaponController.SpawnWeapon(combatAgentId, position, vehicle.WeaponsConfig);
        vehicleWeaponIds.Add(weaponId);
    }

    private void DespawnVehicleAt(int vehicleIndex) {
        vehicles.RemoveAt(vehicleIndex);

        enemyView.RemoveVehicleView(vehicleViewIds[vehicleIndex]);
        vehicleViewIds.RemoveAt(vehicleIndex);

        combatService.UnregisterAgent(vehicleCombats[vehicleIndex]);
        vehicleCombats.RemoveAt(vehicleIndex);

        vehicleService.DeleteVehicle(vehiclePhysics[vehicleIndex]);
        vehiclePhysics.RemoveAt(vehicleIndex);

        soundManager.StopLoop(vehicleSoundLoop[vehicleIndex]);
        vehicleSoundLoop.RemoveAt(vehicleIndex);

        weaponController.DeleteWeapon(vehicleWeaponIds[vehicleIndex]);
        vehicleWeaponIds.RemoveAt(vehicleIndex);
    }

    private void ReadVehiclesOrientation() {
        for (int vehicleIndex = 0; vehicleIndex < vehicles.Count; vehicleIndex++) {
            var vehicle = vehicles[vehicleIndex];
            var physicsId = vehiclePhysics[vehicleIndex];
            
            var vehiclePose = vehicleService.GetVehicleState(physicsId);
            vehicle.UpdatePhysicsState(vehiclePose);

            var weaponId = vehicleWeaponIds[vehicleIndex];
            weaponController.MoveWeapon(weaponId, vehicle.Position);
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
                rewardsMediator.AddReward(vehicle.Position, 3, RewardType.TurelWeapon, vehicle.WeaponsConfig.visualsPrefab.gameObject, new RewardConfigs {
                    weaponConfig = vehicle.WeaponsConfig
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
            enemyView.UpdateVehiclePose(vehicleViewIndex, vehicle.PhysicsState);
        }
    }

}
using UnityEngine;
using System.Collections.Generic;
using Unity.Profiling;
using System.Linq;
using UnityEngine.Assertions;

public class EnemyController {

    private readonly WeaponController weaponController;
    private readonly VehicleController vehicleController;

    private readonly EnemyView enemyView;
    private readonly LocalAvoidanceService localAvoidanceService;
    private readonly NavigationService navigationService;
    private readonly CombatService combatService;
    private readonly PhysicsService physicsService;

    private float lastTimeProduced = float.MinValue;
    private Transform[] spawnPoints;
    private Transform targetPoint;
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

    private readonly VehicleConfig vehicleConfig;
    private readonly WeaponConfig vehicleWeaponConfig;
    private readonly List<EnemyVehicleModel> vehicleModels = new ();

    private readonly List<EnemyVehicleModel> diedVehicles = new ();
    private readonly List<Unit> diedUnits = new ();

    public EnemyController(LocalAvoidanceService localAvoidanceService, NavigationService navigationService, EnemyView crowdView,
        Transform[] spawnPoints, Transform targetPoint, int unitsCount, CombatService combatService, PhysicsService physicsService,
        int maxVehiclesCount, WeaponController weaponController, 
        VehicleController vehicleController, VehicleConfig vehicleConfig, WeaponConfig vehicleWeaponConfig) {
        this.localAvoidanceService = localAvoidanceService;
        this.navigationService = navigationService;
        this.enemyView = crowdView;
        this.spawnPoints = spawnPoints;
        this.targetPoint = targetPoint;
        this.unitsCount = unitsCount;
        this.combatService = combatService;
        this.physicsService = physicsService;

        this.vehiclesSpawnPoints = spawnPoints; // reusing
        this.maxVehiclesCount = maxVehiclesCount;

        this.weaponController = weaponController;
        this.vehicleController = vehicleController;
        this.vehicleConfig = vehicleConfig;
        this.vehicleWeaponConfig = vehicleWeaponConfig;
    }

    public IReadOnlyList<Unit> GetDiedUnits() {
        return diedUnits;
    }

    public IReadOnlyList<EnemyVehicleModel> GetDiedVehicles() {
        return diedVehicles;
    }

    public void ClearDiedRegistry() {
        diedUnits.Clear();
        diedVehicles.Clear();
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
        if (vehicleModels.Count > maxVehiclesCount)
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
        UpdateVehicleNavigation();
        SyncVehiclesPositions();
        ReadVehiclesCombat();
        FilterDeadVehicles();
    }

    private void UpdateGoal() {
        var tractorPosition = vehicleController.GetVehiclePosition(1);
        targetPoint.position = tractorPosition;
        navigationService.SetGoal(tractorPosition);
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

                diedUnits.Add(unit);
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
        var combatAgentId = combatService.RegisterAgent(position, alie: false);
        var model = new EnemyVehicleModel {
            Health = 5,
            Position = position,
            VehicleId = vehicleController.SpawnVehicle(position, vehicleConfig),
            CombatId = combatAgentId,
            WeaponId = weaponController.SpawnWeapon(combatAgentId, position, vehicleWeaponConfig)
        };
        vehicleModels.Add(model);
    }

    private void DespawnVehicleAt(int vehicleIndex) {
        var model = vehicleModels[vehicleIndex];
        vehicleController.DeleteVehicle(model.VehicleId);
        combatService.UnregisterAgent(model.CombatId);
        weaponController.DeleteWeapon(model.WeaponId);
        vehicleModels.RemoveAt(vehicleIndex);
    }

    private void SyncVehiclesPositions() {
        foreach (var model in vehicleModels) {
            model.Position = vehicleController.GetVehiclePosition(model.VehicleId);
            weaponController.MoveWeapon(model.WeaponId, model.Position);
            combatService.UpdateAgentPosition(model.CombatId, model.Position);
        }
    }

    private void ReadVehiclesCombat() {
        foreach (var model in vehicleModels) {
            var vehicleCombatState = combatService.GetAgentState(model.CombatId);
            
            if (vehicleCombatState.projectiled || vehicleCombatState.exploded) {    
                model.Health -= vehicleCombatState.damage;
            }

            combatService.ClearAgentState(model.CombatId);

            if (model.Health <= 0) {
                diedVehicles.Add(model);
            }
        }
    }

    private void FilterDeadVehicles() {
        for (int i = 0; i < vehicleModels.Count; i++) {
            var model = vehicleModels[i];
            if (model.Health <= 0) {
                DespawnVehicleAt(i);
                i--;
            }
        }
    }

    private void UpdateVehicleNavigation() {
        foreach (var model in vehicleModels) {
            var distance = Vector3.Distance(targetPoint.position, model.Position);

            var gasDistance = 10;
            var gas = Mathf.Floor(Mathf.Clamp(distance, 0, gasDistance) / gasDistance);
            vehicleController.DriveVehicle(model.VehicleId, gas, false);
            
            var stopDistance = 5f;
            var brakes = 1 - Mathf.Floor(Mathf.Clamp(distance, 0, stopDistance) / stopDistance);
            vehicleController.BrakeVehicle(model.VehicleId, brakes);

            var flowVector = navigationService.GetFlowVector(model.Position);
            vehicleController.SteerVehicleToward(model.VehicleId, flowVector);
        }
    }

}
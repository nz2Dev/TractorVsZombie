using UnityEngine;
using System.Collections.Generic;
using Unity.Profiling;
using System.Linq;

public class UnitController {

    private readonly UnitView unitView;
    private readonly LocalAvoidanceService localAvoidanceService;
    private readonly NavigationService navigationService;
    private readonly CombatService combatService;
    private readonly PhysicsService physicsService;
    private readonly RewardsMediator rewardsMediator;
    
    private readonly VehicleService vehicleService;
    private readonly VehicleBlueprint foeVehicleBlueprint;
    private readonly VehicleView vehicleView;
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

    private readonly List<Vehicle> vehicles = new List<Vehicle>();
    private readonly List<int> vehiclePhysics = new List<int>();
    private readonly List<int> vehicleCombats = new List<int>();
    private readonly List<int> vehicleSoundLoop = new List<int>();

    public UnitController(LocalAvoidanceService localAvoidanceService, NavigationService navigationService, UnitView crowdView,
        Transform[] spawnPoints, Transform targetPoint, int unitsCount, CombatService combatService, PhysicsService physicsService, RewardsMediator rewardsMediator, VehicleService vehicleService, VehicleBlueprint foeVehicle, int maxVehiclesCount, VehicleView vehicleView, SoundManager soundManager) {
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
        this.foeVehicleBlueprint = foeVehicle;
        this.vehiclesSpawnPoints = spawnPoints; // reusing
        this.maxVehiclesCount = maxVehiclesCount;
        this.vehicleView = vehicleView;
        this.soundManager = soundManager;
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
        if (lastTimeProducedVehicle + 1f > Time.time)
            return;

        lastTimeProducedVehicle = Time.time;
        for (int i = 0; i < vehiclesSpawnPoints.Length; i += 2) {
            if (vehicles.Count > maxVehiclesCount)
                return;
            
            SpawnVehicle(vehiclesSpawnPoints[i].position);
        }
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
        UpdateVehicleNavigation();
        UpdateVehicleCombat();
        UpdateVehiclePhysics();
        UpdateVehiclesSounds();
        UpdateVehiclesView();
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
            
            if (combatAgentState.exploded && unit.TryTakeExplosionHit(Time.time, combatAgentState.damage, combatAgentState.damageSourcePosition)) {
                var unitPhysicsId = unitIdToPhysicsId[unit.Id];
                physicsService.UpdatePhysicsEntityPosition(unitPhysicsId, unit.Position);
                physicsService.SetPhysicsActive(unitPhysicsId, true);
                physicsService.AddExplosionForce(unitPhysicsId, 10, combatAgentState.damageSourcePosition, 4f, 1, ForceMode.Impulse);
                // unitView.ShowTakeExplosionHit(unit.Id);
            }

            if (combatAgentState.projectiled) {
                unit.TakeProjectileHit(combatAgentState.damage, combatAgentState.damageSourcePosition);
                // unitView.ShowTakeProjectileHit(unit.Id, combatAgentState.damageSourcePosition);
            }

            combatService.ClearAgentState(combatId);

            if (!unit.IsAlive) {
                if (unit.DeathCause.type == Unit.DamageType.Projectile) {
                    unitView.ShowDeathByProjectile(unit.Id, unit.DeathCause.damageSource, blownAway: unit.Grouned);
                } else {
                    unitView.ShowDisolveDeath(unit.Id);
                }
                
                combatService.UnregisterAgent(combatId);
                unitIdToCombatId.Remove(unit.Id);

                rewardsMediator.AddReward(unit.Position, radius: 1);
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
        var vehicle = new Vehicle(foeVehicleBlueprint);
        vehicles.Add(vehicle);
        vehicleView.AddVehicle(position, vehicle.PhysicsData, vehicle.VisualsData);

        var combatAgentId = combatService.RegisterAgent(position, alie: false);
        vehicleCombats.Add(combatAgentId);

        var physicsAgentId = vehicleService.CreateVehicle(position, vehicle.PhysicsData);
        vehiclePhysics.Add(physicsAgentId);

        var soundLoopId = soundManager.StartLoop(position, vehicle.EngineIdleSound);
        vehicleSoundLoop.Add(soundLoopId);
    }

    private void ReadVehiclesOrientation() {
        for (int vehicleIndex = 0; vehicleIndex < vehicles.Count; vehicleIndex++) {
            var vehicle = vehicles[vehicleIndex];
            var vehiclePhysicsRigIndex = vehiclePhysics[vehicleIndex];
            
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

    private void UpdateVehicleNavigation() {
        foreach (var vehicle in vehicles) {
            var distance = Vector3.Distance(targetPoint.position, vehicle.BodyPose.position);
            var stopDistance = Mathf.Clamp(distance, 0f, 5f) / 5f;
            vehicle.Throttle(stopDistance, Time.deltaTime, false);
            var flowVector = navigationService.GetFlowVector(vehicle.BodyPose.position);
            vehicle.SteerToward(flowVector);
        }
    }

    private void ReadCombatState() {
        foreach (var vehicle in vehicles) {
            // no health or alive state for vehicle
        }
    }

    private void UpdateVehicleCombat() {
        for (int vehicleIndex = 0; vehicleIndex < vehicles.Count; vehicleIndex++) {
            var vehicle = vehicles[vehicleIndex];
            var vehicelCombatId = vehicleCombats[vehicleIndex];
            combatService.UpdateAgentPosition(vehicelCombatId, vehicle.BodyPose.position);
        }
    }

    private void UpdateVehiclePhysics() {
        for (int vehicleIndex = 0; vehicleIndex < vehicles.Count; vehicleIndex++) {
            var vehicle = vehicles[vehicleIndex];
            var vehiclePhysicId = vehiclePhysics[vehicleIndex];
            vehicleService.SetVehicleEngineTorque(vehiclePhysicId, vehicle.MotorTorque);
            vehicleService.SetVehicleSteer(vehiclePhysicId, vehicle.SteerDegrees);
        }
    }

    private void UpdateVehiclesSounds() {
        for (int vehicleIndex = 0; vehicleIndex < vehicles.Count; vehicleIndex++) {
            var vehicle = vehicles[vehicleIndex];
            var vehicleLoopId = vehicleSoundLoop[vehicleIndex];
            soundManager.UpdateLoop(vehicleLoopId, vehicle.BodyPose.position, vehicle.DrivePower, vehicle.DrivePower);
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

}
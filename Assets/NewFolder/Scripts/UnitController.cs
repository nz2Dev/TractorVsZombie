using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using Unity.Profiling;

public class UnitController {

    private readonly UnitView unitView;
    private readonly LocalAvoidanceService localAvoidanceService;
    private readonly NavigationService navigationService;
    private readonly ICombatService combatService;
    private readonly PhysicsService physicsService;

    private Transform[] spawnPoints;
    private Transform targetPoint;
    private int unitsCount;
    private int idCounter;

    private readonly List<Unit> units = new List<Unit>();
    private readonly Dictionary<int, int> unitIdToAvoidanceId = new Dictionary<int, int>();
    private readonly Dictionary<int, int> unitIdToCombatId = new Dictionary<int, int>();
    private readonly Dictionary<int, int> unitIdToPhysicsId = new Dictionary<int, int>();

    public UnitController(LocalAvoidanceService localAvoidanceService, NavigationService navigationService, UnitView crowdView,
        Transform[] spawnPoints, Transform targetPoint, int unitsCount, ICombatService combatService, PhysicsService physicsService) {
        this.localAvoidanceService = localAvoidanceService;
        this.navigationService = navigationService;
        this.unitView = crowdView;
        this.spawnPoints = spawnPoints;
        this.targetPoint = targetPoint;
        this.unitsCount = unitsCount;
        this.combatService = combatService;
        this.physicsService = physicsService;
    }

    private float lastTimeProduced = float.MinValue;

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

    private void SpawnUnit(Vector3 position) {
        Unit newUnit = new Unit(idCounter++, position, Quaternion.identity, 10f);
        units.Add(newUnit);
        
        var agentId = localAvoidanceService.AddAgent(position);
        unitIdToAvoidanceId[newUnit.Id] = agentId;
        
        var combatAgentId = combatService.RegisterAgent(position);
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
            FilterDeadUnits();
        
        ProduceNewUnits();

        using (readCombatServiceInputMarker.Auto())
            ReadCombatServiceInput();
        using (setCombatStateMarker.Auto())
            SetCombatStateFromUnit();
        using (updateUnitsOrientationMarker.Auto())
            UpdateUnitsOrientation();
        using (updateViewPoseMarker.Auto())
            UpdateViewPose();
    }

    private void ReadUnitOrientation() {
        foreach (var unit in units) {
            var physicsAgentId = unitIdToPhysicsId[unit.Id];
            var avoidanceAgentId = unitIdToAvoidanceId[unit.Id];
            
            var physicsPose = physicsService.GetEntityPose(physicsAgentId);
            var isStartsFlying = unit.Grouned && (physicsPose.InMotion || physicsPose.Pending);
            var keepFlying = !unit.Grouned && physicsPose.InMotion;
            var becomeGrounded = !unit.Grouned && !physicsPose.InMotion;
            var keepsGrouned = unit.Grouned && !physicsPose.InMotion;
            
            if (isStartsFlying) {
                unit.SetFlying();
                unit.Position = physicsPose.Position;
                unit.Rotation = physicsPose.Rotation;
                // localAvoidanceService.SetAgentCollisionEnabled(avoidanceAgentId, false);
            } else if (keepFlying) {
                unit.Position = physicsPose.Position;
                unit.Rotation = physicsPose.Rotation;
            } else if (becomeGrounded) {
                unit.SetGrounded();
                unit.Position = physicsService.GetGroundPosition(physicsPose.Position);
                unit.Rotation = Quaternion.identity;
                // localAvoidanceService.SetAgentCollisionEnabled(avoidanceAgentId, true);
                physicsService.SetPhysicsActive(physicsAgentId, false);
            } else if (keepsGrouned && unit.IsAlive) {
                // unit.Position = localAvoidanceService.GetAgentPosition(avoidanceAgentId);
                // unit.Rotation = localAvoidanceService.GetAgentRotation(avoidanceAgentId);
                localAvoidanceService.GetAgentPositionAndRotation(avoidanceAgentId, out var pos, out var rot);
                unit.Position = pos;
                unit.Rotation = rot;
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
            var combatId = unitIdToCombatId[unit.Id];
            var combatAgentState = combatService.GetAgentState(combatId);
            
            if (combatAgentState.pushed && unit.TryPush(Time.time, combatAgentState.damage)) {
                var unitPhysicsId = unitIdToPhysicsId[unit.Id];
                physicsService.UpdatePhysicsEntityPosition(unitPhysicsId, unit.Position);
                physicsService.SetPhysicsActive(unitPhysicsId, true);
                physicsService.AddExplosionForce(unitPhysicsId, 10, combatAgentState.damageSourcePosition, 1f, 1, ForceMode.Impulse);
            }

            combatService.ClearAgentState(combatId);
        }
    }

    private void SetCombatStateFromUnit() {
        foreach (var unit in units) {
            var combatId = unitIdToCombatId[unit.Id];
            combatService.UpdateAgentPosition(combatId, unit.Position);
        }
    }

    private void FilterDeadUnits() {
        var unitsIndexToRemove = new List<int>();
        for (int i = 0; i < units.Count; i++) {
            var unit = units[i];
            if (!unit.IsAlive && unit.Grouned) {
                unitsIndexToRemove.Add(i);
            }
        }

        foreach (var index in unitsIndexToRemove) {
            DespawnUnitAt(index);
        }
    }

    private void DespawnUnitAt(int unitIndex) {
        var unitId = units[unitIndex].Id;
        units.RemoveAt(unitIndex);
        localAvoidanceService.RemoveAgent(unitIdToAvoidanceId[unitId]);
        unitIdToAvoidanceId.Remove(unitId);
        combatService.UnregisterAgent(unitIdToCombatId[unitId]);
        unitIdToCombatId.Remove(unitId);
        physicsService.UnregisterPhysicsEntity(unitIdToPhysicsId[unitId]);
        unitIdToPhysicsId.Remove(unitId);
        unitView.RemoveUnit(unitId);
    }

    private void UpdateViewPose() {
        foreach (var unit in units) {
            unitView.UpdateUnitPositionAndRotation(unit.Id, unit.Position, unit.Rotation);
        }
    }

}
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class UnitController {

    private readonly UnitView unitView;
    private readonly LocalAvoidanceService localAvoidanceService;
    private readonly NavigationService navigationService;
    private readonly CombatService combatService;
    private readonly PhysicsService physicsService;

    private Transform[] spawnPoints;
    private Transform targetPoint;
    private int unitsCount;

    private readonly List<Unit> units = new List<Unit>();
    private readonly Dictionary<int, int> unitIdToAvoidanceId = new Dictionary<int, int>();
    private readonly Dictionary<int, int> unitIdToCombatId = new Dictionary<int, int>();
    private readonly Dictionary<int, int> unitIdToPhysicsId = new Dictionary<int, int>();

    public UnitController(LocalAvoidanceService localAvoidanceService, NavigationService navigationService, UnitView crowdView,
        Transform[] spawnPoints, Transform targetPoint, int unitsCount, CombatService combatService, PhysicsService physicsService) {
        this.localAvoidanceService = localAvoidanceService;
        this.navigationService = navigationService;
        this.unitView = crowdView;
        this.spawnPoints = spawnPoints;
        this.targetPoint = targetPoint;
        this.unitsCount = unitsCount;
        this.combatService = combatService;
        this.physicsService = physicsService;
    }

    public IEnumerator Initialize() {
        yield return AddsNewUnitsEachFrameForFixedTime();
    }

    private IEnumerator AddsNewUnitsEachFrameForFixedTime() {
        for (int i = 0; i < unitsCount; i++) {
            foreach (var spawnPoint in spawnPoints) {    
                SpawnUnit(spawnPoint.position);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }

    private void SpawnUnit(Vector3 position) {
        Unit newUnit = new Unit(units.Count, position, Quaternion.identity, 10f);
        units.Add(newUnit);
        
        var agentId = localAvoidanceService.AddAgent(position);
        unitIdToAvoidanceId[newUnit.Id] = agentId;
        
        var combatAgentId = combatService.RegisterCombatant(0.3f, position, physicalDamage: 10);
        unitIdToCombatId[newUnit.Id] = combatAgentId;

        var physicsAgentId = physicsService.RegisterPhysicsEntity(position, .5f, 0.15f);
        unitIdToPhysicsId[newUnit.Id] = physicsAgentId;
        
        unitView.AddUnit(newUnit.Id, position);
    }

    public void Update() {
        FilterDeadUnits();
        ReadUnitOrientation();
        ReadCombatServiceInput();
        SetCombatStateFromUnit();
        UpdateUnitsOrientation();
        UpdateViewPose();
    }

    private void ReadUnitOrientation() {
        foreach (var unit in units) {
            var physicsAgentId = unitIdToPhysicsId[unit.Id];
            var avoidanceAgentId = unitIdToAvoidanceId[unit.Id];
            var physicsPose = physicsService.GetEntityPose(physicsAgentId);
            
            var isStartsFlying = unit.Grouned && physicsPose.InMotion;
            var keepFlying = !unit.Grouned && physicsPose.InMotion;
            var becomeGrounded = !unit.Grouned && !physicsPose.InMotion;
            var keepsGrouned = unit.Grouned && !physicsPose.InMotion;
            
            if (isStartsFlying) {
                unit.SetFlying();
                unit.Position = physicsPose.Position;
                // localAvoidanceService.SetAgentCollisionEnabled(avoidanceAgentId, false);
            } else if (keepFlying) {
                unit.Position = physicsPose.Position;
            } else if (becomeGrounded) {
                unit.SetGrounded();
                unit.Position = physicsPose.Position;
                // localAvoidanceService.SetAgentCollisionEnabled(avoidanceAgentId, true);
                physicsService.SetPhysicsActive(physicsAgentId, false);
            } else if (keepsGrouned) {
                unit.Position = localAvoidanceService.GetAgentPosition(avoidanceAgentId);
                unit.Rotation = localAvoidanceService.GetAgentRotation(avoidanceAgentId);
            }
        }
    }

    private void UpdateUnitsOrientation() {
        navigationService.SetGoal(targetPoint.position);
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
            var combatState = combatService.GetState(combatId);
            
            if (unit.Grouned && combatState.pushed) {
                // unit.TakeDamage(1);
                var unitPhysicsId = unitIdToPhysicsId[unit.Id];
                physicsService.UpdatePhysicsEntityPosition(unitPhysicsId, unit.Position);
                physicsService.SetPhysicsActive(unitPhysicsId, true);
                physicsService.AddExplosionForce(unitPhysicsId, 10, combatState.pushEpicenter, 1f, 1, ForceMode.Impulse);
            }

            combatService.ClearState(combatId);
        }
    }

    private void SetCombatStateFromUnit() {
        foreach (var unit in units) {
            var combatId = unitIdToCombatId[unit.Id];
            combatService.UpdateAgentPosition(combatId, unit.Position);
            combatService.UpdateAgentAOEDiscoverable(combatId, unit.Grouned);
        }
    }

    private void FilterDeadUnits() {
        var unitsToRemove = new List<int>();
        foreach (var unit in units) {
            if (!unit.IsAlive) {
                unitsToRemove.Add(unit.Id);
            }
        }

        foreach (var id in unitsToRemove) {
            DespawnUnit(id);
        }
    }

    private void DespawnUnit(int id) {
        units.RemoveAll(u => u.Id == id);
        // localAvoidanceService.RemoveAgent(unitIdToAvoidanceId[id]);
        unitIdToAvoidanceId.Remove(id);
        combatService.UnregisterAgent(unitIdToCombatId[id]);
        unitIdToCombatId.Remove(id);
        physicsService.UnregisterPhysicsEntity(unitIdToPhysicsId[id]);
        unitIdToPhysicsId.Remove(id);
        unitView.RemoveUnit(id);
    }

    private void UpdateViewPose() {
        foreach (var unit in units) {
            unitView.UpdateUnitPositionAndRotation(unit.Id, unit.Position, unit.Rotation);
        }
    }

}
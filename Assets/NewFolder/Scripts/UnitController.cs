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

    private Transform spawnPoint;
    private Transform targetPoint;
    private int unitsCount;

    private readonly List<Unit> units = new List<Unit>();
    private readonly Dictionary<int, int> unitIdToAvoidanceId = new Dictionary<int, int>();
    private readonly Dictionary<int, int> unitIdToCombatId = new Dictionary<int, int>();
    private readonly Dictionary<int, int> unitIdToPhysicsId = new Dictionary<int, int>();

    public UnitController(LocalAvoidanceService localAvoidanceService, NavigationService navigationService, UnitView crowdView,
        Transform spawnPoint, Transform targetPoint, int unitsCount, CombatService combatService, PhysicsService physicsService) {
        this.localAvoidanceService = localAvoidanceService;
        this.navigationService = navigationService;
        this.unitView = crowdView;
        this.spawnPoint = spawnPoint;
        this.targetPoint = targetPoint;
        this.unitsCount = unitsCount;
        this.combatService = combatService;
        this.physicsService = physicsService;
    }

    public IEnumerator Initialize() {
        yield return AddsNewUnitsEachFrameForFixedTime();
    }

    public void Update() {
        FilterDeadUnits();
        ReadUnitOrientation();
        ReadCombatServiceInput();
        SetCombatStateFromUnit();
        UpdateUnitsNavigation();
        UpdateViewPose();
    }

    private void UpdateUnitsNavigation() {
        navigationService.SetGoal(targetPoint.position);
        foreach (var unit in units) {
            if (!unit.Flying) {
                var avoidanceAgentId = unitIdToAvoidanceId[unit.Id];
                localAvoidanceService.SetAgentPosition(avoidanceAgentId, unit.Position);
                
                var flowVector = navigationService.GetFlowVector(unit.Position);
                localAvoidanceService.SetPreferedVelocity(avoidanceAgentId, flowVector);
            }
        }
    }

    private void ReadUnitOrientation() {
        foreach (var unit in units) {
            if (unit.Flying) {
                var unitPhysicsId = unitIdToPhysicsId[unit.Id];
                var physicsPose = physicsService.GetEntityPose(unitPhysicsId);
                unit.Position = physicsPose.Position;
                if (!physicsPose.InMotion) {
                    unit.Walking();
                }
            } else {
                var avoidanceAgentId = unitIdToAvoidanceId[unit.Id];
                unit.Position = localAvoidanceService.GetAgentPosition(avoidanceAgentId);
                unit.Rotation = localAvoidanceService.GetAgentRotation(avoidanceAgentId);
            }
        }
    }

    private IEnumerator AddsNewUnitsEachFrameForFixedTime() {
        for (int i = 0; i < unitsCount; i++) {
            SpawnUnit(spawnPoint.position);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void SpawnUnit(Vector3 position) {
        Unit newUnit = new Unit(units.Count, position, Quaternion.identity, 10f);
        units.Add(newUnit);
        
        var agentId = localAvoidanceService.AddAgent(position);
        unitIdToAvoidanceId[newUnit.Id] = agentId;
        
        var combatAgentId = combatService.RegisterCombatant(0.5f, position, physicalDamage: 10);
        unitIdToCombatId[newUnit.Id] = combatAgentId;

        var physicsAgentId = physicsService.RegisterPhysicsEntity(position, .3f, 0.5f);
        unitIdToPhysicsId[newUnit.Id] = physicsAgentId;
        
        unitView.AddUnit(newUnit.Id, position);
    }

    private void ReadCombatServiceInput() {
        foreach (var unit in units) {
            var combatId = unitIdToCombatId[unit.Id];
            var combatState = combatService.GetState(combatId);
            if (combatState.damageReceived > 0) {
                // unit.TakeDamage((int) combatState.damageReceived);
                Debug.Log($"unit {unit.Id} receive {combatState.damageReceived} damage");
            }
            
            if (combatState.pushed) {
                var unitPhysicsId = unitIdToPhysicsId[unit.Id];
                physicsService.UpdatePhysicsEntityPosition(unitPhysicsId, unit.Position);
                physicsService.SetPhysicsActive(unitPhysicsId, true);
                physicsService.AddExplosionForce(unitPhysicsId, 2, combatState.pushEpicenter, 1f, 1, ForceMode.Impulse);
                unit.SetFlying();
            }

            combatService.ClearState(combatId);
        }
    }

    private void SetCombatStateFromUnit() {
        foreach (var unit in units) {
            var combatId = unitIdToCombatId[unit.Id];
            combatService.UpdateAgentPosition(combatId, unit.Position);
            if (unit.Flying) {
                combatService.UpdateAgentDiscovered(combatId, false);
            }
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
        unitIdToAvoidanceId.Remove(unitIdToAvoidanceId[id]);
        combatService.UnregisterAgent(unitIdToCombatId[id]);
        physicsService.UnregisterPhysicsEntity(unitIdToPhysicsId[id]);
        unitView.RemoveUnit(id);
    }

    private void UpdateViewPose() {
        foreach (var unit in units) {
            unitView.UpdateUnitPositionAndRotation(unit.Id, unit.Position, unit.Rotation);
        }
    }

}
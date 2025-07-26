using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class UnitController {

    private readonly UnitView unitView;
    private readonly LocalAvoidanceService localAvoidanceService;
    private readonly NavigationService navigationService;
    private readonly PhysicsService physicsService;

    private Transform spawnPoint;
    private Transform targetPoint;
    private int unitsCount;

    private readonly List<Unit> units = new List<Unit>();
    private readonly Dictionary<int, int> agentIdToUnitId = new Dictionary<int, int>();

    public UnitController(LocalAvoidanceService localAvoidanceService, NavigationService navigationService, PhysicsService physicsService, UnitView crowdView, 
        Transform spawnPoint, Transform targetPoint, int unitsCount) {
        this.localAvoidanceService = localAvoidanceService;
        this.navigationService = navigationService;
        this.unitView = crowdView;
        this.spawnPoint = spawnPoint;
        this.targetPoint = targetPoint;
        this.unitsCount = unitsCount;
        this.physicsService = physicsService;
    }

    public IEnumerator Initialize() {
        yield return AddsNewUnitsEachFrameForFixedTime();
    }

    public void Update() {
        PerformKillZoneDamage();
        FilterDeadUnits();
        SetUnitPoseFromAvoidanceService();
        NavigateLocalAvoidanceService();
        SetPhysicsStateFromUnit();
        UpdateViewPose();
    }

    private IEnumerator AddsNewUnitsEachFrameForFixedTime() {
        for (int i = 0; i < unitsCount; i++) {
            SpawnUnit(spawnPoint.position);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void PerformKillZoneDamage() {
        var battlegroundKillZone = targetPoint.position;
        var battlegroundKillZoneRadius = 1f;
        
        var unitsInKillZone = physicsService.QuerySphere(battlegroundKillZone, battlegroundKillZoneRadius);
        foreach (var unitId in unitsInKillZone) {
            var unit = units.Find(u => u.Id == unitId);
            unit.ForceKill();
        }
    }

    private void SpawnUnit(Vector3 position) {
        Unit newUnit = new Unit(units.Count, position, Quaternion.identity, 10f);
        units.Add(newUnit);
        
        var agentId = localAvoidanceService.AddAgent(position);
        agentIdToUnitId[agentId] = newUnit.Id;
        
        physicsService.RegisterPhysicsEntity(newUnit.Id, position, 1, 1);
        
        unitView.AddUnit(newUnit.Id, position);
    }

    private void NavigateLocalAvoidanceService() {
        navigationService.SetGoal(targetPoint.position);
        foreach (var unit in units) {
            var flowVector = navigationService.GetFlowVector(unit.Position);
            var unitAgentId = agentIdToUnitId[unit.Id];
            localAvoidanceService.SetPreferedVelocity(unitAgentId, flowVector);
        }
    }

    private void SetPhysicsStateFromUnit() {
        foreach (var unit in units) {
            physicsService.UpdatePhysicsEntityPosition(unit.Id, unit.Position);
        }
    }

    private void SetUnitPoseFromAvoidanceService() {
        foreach (var unit in units) {
            var unitAgentId = agentIdToUnitId[unit.Id];
            unit.Position = localAvoidanceService.GetAgentPosition(unitAgentId);
            unit.Rotation = localAvoidanceService.GetAgentRotation(unitAgentId);
        }
    }

    private void UpdateViewPose() {
        foreach (var unit in units) {
            unitView.UpdateUnitPositionAndRotation(unit.Id, unit.Position, unit.Rotation);
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
        agentIdToUnitId.Remove(id);
        physicsService.UnregisterPhysicsEntity(id);
        unitView.RemoveUnit(id);
    }

}
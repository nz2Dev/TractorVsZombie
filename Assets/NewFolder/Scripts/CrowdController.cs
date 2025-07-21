using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CrowdController {

    private CrowdView crowdView;
    private LocalAvoidanceService localAvoidanceService;
    private NavigationService navigationService;
    private PhysicsService physicsService;

    private Transform spawnPoint;
    private Transform targetPoint;
    private int unitsCount;

    private readonly List<CrowdUnit> crowdUnits = new List<CrowdUnit>();
    private readonly Dictionary<int, int> agentIdToCrowdUnitId = new Dictionary<int, int>();

    public CrowdController(LocalAvoidanceService localAvoidanceService, NavigationService navigationService, PhysicsService physicsService, CrowdView crowdView, 
        Transform spawnPoint, Transform targetPoint, int unitsCount) {
        this.localAvoidanceService = localAvoidanceService;
        this.navigationService = navigationService;
        this.crowdView = crowdView;
        this.spawnPoint = spawnPoint;
        this.targetPoint = targetPoint;
        this.unitsCount = unitsCount;
        this.physicsService = physicsService;
    }

    public IEnumerator Initialize() {
        for (int i = 0; i < unitsCount; i++) {
            SpawnCrowdUnit(spawnPoint.position);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void Update() {
        CoordinateCrowdUnits();
        UpdateBattleground();
        UpdateCrowdUnits();
    }

    private void UpdateBattleground() {
        var battlegroundKillZone = targetPoint.position;
        var battlegroundKillZoneRadius = 1f;
        
        var unitsInKillZone = physicsService.QuerySphere(battlegroundKillZone, battlegroundKillZoneRadius);
        foreach (var unitId in unitsInKillZone) {
            var unit = crowdUnits.Find(u => u.Id == unitId);
            unit.ForceKill();
        }
    }

    private void SpawnCrowdUnit(Vector3 position) {
        CrowdUnit newUnit = new CrowdUnit(crowdUnits.Count, position, Quaternion.identity, 10f);
        crowdUnits.Add(newUnit);
        
        var agentId = localAvoidanceService.AddAgent(position);
        agentIdToCrowdUnitId[agentId] = newUnit.Id;
        
        physicsService.RegisterPhysicsEntity(newUnit.Id, position, 1, 1);
        
        crowdView.AddUnit(newUnit.Id, position);
    }

    private void CoordinateCrowdUnits() {
        navigationService.SetGoal(targetPoint.position);
        foreach (var unit in crowdUnits) {
            var flowVector = navigationService.GetFlowVector(unit.Position);
            var unitAgentId = agentIdToCrowdUnitId[unit.Id];
            localAvoidanceService.SetPreferedVelocity(unitAgentId, flowVector);
        }
    }

    private void UpdateCrowdUnits() {
        var unitsToRemove = new List<int>();
        
        foreach (var unit in crowdUnits) {
            var unitAgentId = agentIdToCrowdUnitId[unit.Id];
            unit.Position = localAvoidanceService.GetAgentPosition(unitAgentId);
            unit.Rotation = localAvoidanceService.GetAgentRotation(unitAgentId);
            physicsService.UpdatePhysicsEntityPosition(unit.Id, unit.Position);
            crowdView.UpdateUnitPositionAndRotation(unit.Id, unit.Position, unit.Rotation);
            
            if (!unit.IsAlive) {
                unitsToRemove.Add(unit.Id);
            }
        }

        foreach (var id in unitsToRemove) {
            DespawnCrowdUnit(id);
        }
    }

    private void DespawnCrowdUnit(int id) {
        crowdUnits.RemoveAll(u => u.Id == id);
        agentIdToCrowdUnitId.Remove(id);
        physicsService.UnregisterPhysicsEntity(id);
        crowdView.RemoveUnit(id);
    }

}
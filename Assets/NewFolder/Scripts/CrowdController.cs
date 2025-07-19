using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CrowdController {

    private CrowdView crowdView;
    private LocalAvoidanceService localAvoidanceService;
    private NavigationService navigationService;

    private Transform spawnPoint;
    private Transform targetPoint;
    private int unitsCount;

    private readonly List<CrowdUnit> crowdUnits = new List<CrowdUnit>();
    private readonly Dictionary<int, int> agentIdToCrowdUnitId = new Dictionary<int, int>();

    public CrowdController(LocalAvoidanceService localAvoidanceService, NavigationService navigationService, CrowdView crowdView, 
        Transform spawnPoint, Transform targetPoint, int unitsCount) {
        this.localAvoidanceService = localAvoidanceService;
        this.navigationService = navigationService;
        this.crowdView = crowdView;
        this.spawnPoint = spawnPoint;
        this.targetPoint = targetPoint;
        this.unitsCount = unitsCount;
    }

    public IEnumerator Initialize() {
        for (int i = 0; i < unitsCount; i++) {
            SpawnCrowdUnit(spawnPoint.position);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void Update() {
        CoordinateCrowdUnits();
        UpdateCrowdUnits();
    }

    private void SpawnCrowdUnit(Vector3 position) {
        CrowdUnit newUnit = new CrowdUnit(crowdUnits.Count, position, Quaternion.identity, 10f);
        crowdUnits.Add(newUnit);
        
        var agentId = localAvoidanceService.AddAgent(position);
        agentIdToCrowdUnitId[agentId] = newUnit.Id;
        
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
        foreach (var unit in crowdUnits) {
            var unitAgentId = agentIdToCrowdUnitId[unit.Id];
            unit.Position = localAvoidanceService.GetAgentPosition(unitAgentId);
            unit.Rotation = localAvoidanceService.GetAgentRotation(unitAgentId);
            crowdView.UpdateUnitPositionAndRotation(unit.Id, unit.Position, unit.Rotation);
        }
    }
}
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class UnitController {

    private readonly UnitView unitView;
    private readonly LocalAvoidanceService localAvoidanceService;
    private readonly NavigationService navigationService;
    private readonly CombatService combatService;

    private Transform spawnPoint;
    private Transform targetPoint;
    private int unitsCount;

    private readonly List<Unit> units = new List<Unit>();
    private readonly Dictionary<int, int> agentIdToUnitId = new Dictionary<int, int>();
    private readonly Dictionary<int, int> unitIdToCombatId = new Dictionary<int, int>();

    public UnitController(LocalAvoidanceService localAvoidanceService, NavigationService navigationService, UnitView crowdView,
        Transform spawnPoint, Transform targetPoint, int unitsCount, CombatService combatService) {
        this.localAvoidanceService = localAvoidanceService;
        this.navigationService = navigationService;
        this.unitView = crowdView;
        this.spawnPoint = spawnPoint;
        this.targetPoint = targetPoint;
        this.unitsCount = unitsCount;
        this.combatService = combatService;
    }

    public IEnumerator Initialize() {
        yield return AddsNewUnitsEachFrameForFixedTime();
    }

    public void Update() {
        FilterDeadUnits();
        SetUnitPoseFromAvoidanceService();
        NavigateLocalAvoidanceService();
        ReadCombatServiceInput();
        SetCombatStateFromUnit();
        UpdateViewPose();
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
        agentIdToUnitId[agentId] = newUnit.Id;
        
        var combatAgentId = combatService.RegisterCombatant(0.5f, position, physicalDamage: 10);
        unitIdToCombatId[newUnit.Id] = combatAgentId;
        
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

    private void ReadCombatServiceInput() {
        foreach (var unit in units) {
            var combatId = unitIdToCombatId[unit.Id];
            var combatState = combatService.GetState(combatId);
            if (combatState.damageReceived > 0) {
                unit.TakeDamage((int) combatState.damageReceived);
                Debug.Log($"unit {unit.Id} receive {combatState.damageReceived} damage");
            }
        }
    }

    private void SetCombatStateFromUnit() {
        foreach (var unit in units) {
            var combatId = unitIdToCombatId[unit.Id];
            combatService.UpdateAgentPosition(combatId, unit.Position);
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
        combatService.UnregisterAgent(unitIdToCombatId[id]);
        unitView.RemoveUnit(id);
    }

}
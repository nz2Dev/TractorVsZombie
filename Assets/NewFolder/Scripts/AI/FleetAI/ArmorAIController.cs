using System;
using System.Collections.Generic;

using Combat;

using UnityEngine;

public class ArmorAIController {

    private readonly CombatSystem combatSystem;
    private readonly PathfindingService pathfindingService;
    private readonly ArmorController armorController;
    private readonly WeaponController weaponController;
    private readonly ProximityService proximityService;

    private readonly List<int> controlledArmorIds = new();
    private readonly int flowFieldId;

    public ArmorAIController(CombatSystem combatSystem, PathfindingService pathfindingService, ArmorController armorController, WeaponController weaponController, ProximityService proximityService) {
        this.combatSystem = combatSystem;
        this.pathfindingService = pathfindingService;
        this.armorController = armorController;
        this.weaponController = weaponController;
        this.proximityService = proximityService;

        flowFieldId = pathfindingService.CreateFlowField(Vector3.zero);
    }

    public void Update() {
        ValidateArmorIds();
        OperateArmors();
    }

    public void AddAIBehaviour(int armorId) {
        controlledArmorIds.Add(armorId);
    }

    private void ValidateArmorIds() {
        armorController.WriteDeadArmorFiltered(controlledArmorIds);
    }

    private void OperateArmors() {
        foreach (var armorId in controlledArmorIds) {
            var state = armorController.ReadArmorState(armorId);
            OperateArmorNavigation(armorId, state);
            OperateArmorCombat(state);
        }
    }

    private void OperateArmorNavigation(int armorId, ArmorState state) {
        var goal = Vector3.zero;
        var distance = Vector3.Distance(goal, state.position);

        var gasDistance = 10;
        var gas = Mathf.Floor(Mathf.Clamp(distance, 0, gasDistance) / gasDistance);
        armorController.Drive(armorId, gas, false);

        var stopDistance = 5f;
        var brakes = 1 - Mathf.Floor(Mathf.Clamp(distance, 0, stopDistance) / stopDistance);
        armorController.Brake(armorId, brakes);

        var flowVector = pathfindingService.GetFlowVector(flowFieldId, state.position);
        armorController.SteerToward(armorId, flowVector);
    }

    private void OperateArmorCombat(ArmorState state) {
        var enemySearchRadius = state.weaponState.aimConfig.range;

        var targetFaction = !state.combatIsAlie;
        var targetProximityLayer = CombatSystem.GetProximityLayerForFaction(targetFaction);
        if (proximityService.QueryNearestPoint(state.position, targetProximityLayer, out var nearestProximityId)) {
            // todo: either find mapped raycastId and het height from there, or else?
            var pointPosition = proximityService.GetPoint(nearestProximityId);
            weaponController.AimWeapon(state.weaponId, pointPosition + 0.5f * Vector3.up);
        }
    }

}

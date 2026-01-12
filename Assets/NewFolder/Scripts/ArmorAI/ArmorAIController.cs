using System;
using System.Collections.Generic;

using UnityEngine;

public class ArmorAIController {
    
    private readonly CombatService combatService;
    private readonly PathfindingService pathfindingService;
    private readonly ArmorController armorController;
    private readonly MotorVehicleController motorVehicleController;
    private readonly WeaponController weaponController;

    private Vector3 goal;
    private readonly List<int> controlledArmorIds = new();

    public ArmorAIController(CombatService combatService, PathfindingService pathfindingService, ArmorController armorController, MotorVehicleController motorVehicleController, WeaponController weaponController) {
        this.combatService = combatService;
        this.pathfindingService = pathfindingService;
        this.armorController = armorController;
        this.motorVehicleController = motorVehicleController;
        this.weaponController = weaponController;
    }

    public void Update() {
        ValidateArmorIds();
        OperateArmors();
    }

    public void SetGoal(Vector3 goal) {
        this.goal = goal;
        pathfindingService.SetGoal(goal);
    }

    public void TakeUnderControl(int armorId) {
        controlledArmorIds.Add(armorId);
    }

    private void ValidateArmorIds() {
        armorController.WriteDeadArmorFiltered(controlledArmorIds);
    }

    private void OperateArmors() {
        foreach (var armorId in controlledArmorIds) {
            var state = armorController.ReadArmorState(armorId);
            OperateArmorNavigation(state);
            OperateArmorCombat(state);
        }
    }

    private void OperateArmorNavigation(ArmorState state) {
        var distance = Vector3.Distance(goal, state.position);

        var gasDistance = 10;
        var gas = Mathf.Floor(Mathf.Clamp(distance, 0, gasDistance) / gasDistance);
        motorVehicleController.DriveVehicle(state.vehicleId, gas, false);
        
        var stopDistance = 5f;
        var brakes = 1 - Mathf.Floor(Mathf.Clamp(distance, 0, stopDistance) / stopDistance);
        motorVehicleController.BrakeVehicle(state.vehicleId, brakes);

        var flowVector = pathfindingService.GetFlowVector(state.position);
        motorVehicleController.SteerVehicleToward(state.vehicleId, flowVector);
    }

    private void OperateArmorCombat(ArmorState state) {
        var enemySearchRadius = state.weaponConfig.aimConfig.range;
        if (combatService.GetClosestEnemyAgentInRange(state.combatId, enemySearchRadius, out var agentInfo)) {
            weaponController.AimWeapon(state.weaponId, agentInfo.position + 0.5f * agentInfo.height * Vector3.up);
        }
    }

}
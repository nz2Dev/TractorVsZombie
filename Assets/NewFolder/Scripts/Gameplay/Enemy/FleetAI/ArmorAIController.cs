using System;
using System.Collections.Generic;


using UnityEngine;

public class ArmorAIController {

    private readonly HealthSystem healthSystem;
    private readonly PathfindingService pathfindingService;
    private readonly ArmorController armorController;
    private readonly VehicleSystem vehicleSystem;
    private readonly WeaponController weaponController;
    private readonly HitboxSystem hitboxSystem;

    private readonly List<int> controlledArmorIds = new();
    private readonly int flowFieldId;

    public ArmorAIController(HealthSystem healthSystem, PathfindingService pathfindingService, ArmorController armorController, VehicleSystem vehicleSystem, WeaponController weaponController, HitboxSystem hitboxSystem) {
        this.healthSystem = healthSystem;
        this.pathfindingService = pathfindingService;
        this.armorController = armorController;
        this.vehicleSystem = vehicleSystem;
        this.weaponController = weaponController;
        this.hitboxSystem = hitboxSystem;

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
            OperateArmorNavigation(state);
            OperateArmor(state);
        }
    }

    private void OperateArmorNavigation(ArmorState state) {
        var goal = Vector3.zero;
        var distance = Vector3.Distance(goal, state.position);

        var gasDistance = 10;
        var gas = Mathf.Floor(Mathf.Clamp(distance, 0, gasDistance) / gasDistance);
        vehicleSystem.SetGas(state.vehicleId, gas);

        var stopDistance = 5f;
        var brakes = 1 - Mathf.Floor(Mathf.Clamp(distance, 0, stopDistance) / stopDistance);
        vehicleSystem.SetBrakes(state.vehicleId, brakes);

        var flowVector = pathfindingService.GetFlowVector(flowFieldId, state.position);
        var vehicleState = vehicleSystem.ReadState(state.vehicleId);
        var forward = vehicleState.pose.rotation * Vector3.forward;
        var forwardToDirectionDegrees = Vector3.SignedAngle(forward, flowVector, Vector3.up);
        vehicleSystem.SetSteerAngle(state.vehicleId, forwardToDirectionDegrees);
    }

    private void OperateArmor(ArmorState state) {
        if (hitboxSystem.QueryNearest(state.position, !state.factionIsAlie, out var nearestHitboxId)) {
            // todo: either find mapped raycastId and het height from there, or else?
            var pointPosition = hitboxSystem.GetPosition(nearestHitboxId);
            weaponController.AimWeapon(state.attachmentState.weaponId, pointPosition + 0.5f * Vector3.up);
        }
    }

}

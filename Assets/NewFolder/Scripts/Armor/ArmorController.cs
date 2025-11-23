using System.Collections.Generic;

using UnityEngine;

public class ArmorController {

    private readonly CombatService combatService;
    private readonly NavigationService navigationService;
    private readonly WeaponController weaponController;
    private readonly VehicleController vehicleController;

    private int idCounter;    
    private readonly Dictionary<int, ArmorModel> registry = new ();
    private readonly List<ArmorModel> diedArmor = new ();

    public ArmorController(CombatService combatService, NavigationService navigationService, WeaponController weaponController, VehicleController vehicleController) {
        this.combatService = combatService;
        this.navigationService = navigationService;
        this.weaponController = weaponController;
        this.vehicleController = vehicleController;
    }

    public int ArmorCount => registry.Count;
    public IReadOnlyList<ArmorModel> DiedArmor => diedArmor;
    public void ClearDiedRegistry() => diedArmor.Clear();

    public void Update() {
        ReadVehiclesCombat();
        RemoveDeadArmor();
        SyncVehiclesPositions();
        // todo implemet operate weapons here and remove from weapons controller
        UpdateVehicleNavigation();
    }

    public int SpawnArmor(Vector3 position, ArmorConfig armorConfig) {
        var nextId = ++idCounter;
        var model = new ArmorModel(nextId, position, armorConfig);
        registry[model.Id] = model;
        model.CombatId = combatService.RegisterAgent(position, alie: false);
        model.VehicleId = vehicleController.SpawnVehicle(position, model.VehicleConfig);
        model.WeaponId = weaponController.SpawnWeapon(model.CombatId, position, model.WeaponConfig);
        model.Health = model.MaxHealth;
        return model.Id;
    }

    private void DeleteArmor(ArmorModel model) {
        vehicleController.DeleteVehicle(model.VehicleId);
        combatService.UnregisterAgent(model.CombatId);
        weaponController.DeleteWeapon(model.WeaponId);
        registry.Remove(model.Id);
    }

    private void ReadVehiclesCombat() {
        foreach (var model in registry.Values) {
            var combatState = combatService.GetAgentState(model.CombatId);
            
            if (combatState.projectiled || combatState.exploded) {    
                model.Health -= combatState.damage;
            }

            combatService.ClearAgentState(model.CombatId);

            if (model.Health <= 0) {
                diedArmor.Add(model);
            }
        }
    }

    private void RemoveDeadArmor() {
        List<ArmorModel> removeBuffer = new();
        foreach (var model in registry.Values) {
            if (model.Health <= 0) {
                removeBuffer.Add(model);
            }
        }
        foreach (var model in removeBuffer) {
            DeleteArmor(model);
        }
    }

    private void SyncVehiclesPositions() {
        foreach (var model in registry.Values) {
            model.Position = vehicleController.GetVehiclePosition(model.VehicleId);
            weaponController.MoveWeapon(model.WeaponId, model.Position);
            combatService.UpdateAgentPosition(model.CombatId, model.Position);
        }
    }

    private void UpdateVehicleNavigation() {
        foreach (var model in registry.Values) {
            var distance = Vector3.Distance(navigationService.GetGoal(), model.Position);

            var gasDistance = 10;
            var gas = Mathf.Floor(Mathf.Clamp(distance, 0, gasDistance) / gasDistance);
            vehicleController.DriveVehicle(model.VehicleId, gas, false);
            
            var stopDistance = 5f;
            var brakes = 1 - Mathf.Floor(Mathf.Clamp(distance, 0, stopDistance) / stopDistance);
            vehicleController.BrakeVehicle(model.VehicleId, brakes);

            var flowVector = navigationService.GetFlowVector(model.Position);
            vehicleController.SteerVehicleToward(model.VehicleId, flowVector);
        }
    }
}
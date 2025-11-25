using System;
using System.Collections.Generic;

using UnityEngine;

public class ArmorController {

    private readonly SoundManager soundManager;
    private readonly CombatService combatService;
    private readonly WeaponController weaponController;
    private readonly VehicleController vehicleController;

    private int idCounter;    
    private readonly Dictionary<int, ArmorModel> registry = new ();
    private readonly List<ArmorModel> diedArmor = new ();

    public ArmorController(SoundManager soundManager, CombatService combatService, WeaponController weaponController, VehicleController vehicleController) {
        this.combatService = combatService;
        this.weaponController = weaponController;
        this.vehicleController = vehicleController;
        this.soundManager = soundManager;
    }

    public int ArmorCount => registry.Count;
    public IReadOnlyList<ArmorModel> DiedArmor => diedArmor;
    public void ClearDiedRegistry() => diedArmor.Clear();

    public void Update() {
        ReadVehiclesCombat();
        RemoveDeadArmor();
        SyncVehiclesPositions();
        ComputeRamDamage();
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

    public void WriteDeadArmorFiltered(List<int> armorIds) {
        armorIds.RemoveAll(id => !registry.ContainsKey(id));
    }
    
    public ArmorState ReadArmorState(int armorId) {
        var armor = registry[armorId];
        return new ArmorState {
            position = armor.Position,
            combatId = armor.CombatId,
            vehicleId = armor.VehicleId,
            weaponId = armor.WeaponId,
            weaponConfig = armor.WeaponConfig
        };
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

    private void ComputeRamDamage() {
        foreach (var armor in registry.Values) {
            if (!armor.CanApplyRamDamage)
                continue;

            var affectedCount = combatService.ApplyExplosionDamage(armor.CombatId, armor.Position, armor.RamRadius, damage: 0);
            for (int i = 0; i < affectedCount; i++) {
                var position = armor.Position + UnityEngine.Random.onUnitSphere * armor.RamRadius;
                soundManager.PlayEffectDelayed(position, i * 0.05f, armor.RamImpactSFX);
            }    
        }
    }

}
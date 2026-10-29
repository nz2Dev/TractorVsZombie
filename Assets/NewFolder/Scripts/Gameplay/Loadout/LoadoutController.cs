using System;
using System.Collections.Generic;


using UnityEngine;

public class LoadoutController {

    private readonly LoadoutView view;
    private readonly WeaponController weaponController;
    private readonly HealthSystem healthSystem;
    private readonly HitboxSystem hitboxSystem;
    private readonly EntityMapping entityMapping;
    private readonly FactionRegistry factionRegistry;

    private int idCounter;
    private readonly Dictionary<int, LoadoutModel> registry = new ();

    public LoadoutController(LoadoutView view, WeaponController weaponController, HealthSystem healthSystem, FactionRegistry factionRegistry, HitboxSystem hitboxSystem, EntityMapping entityMapping) {
        this.view = view;
        this.weaponController = weaponController;
        this.healthSystem = healthSystem;
        this.factionRegistry = factionRegistry;
        this.hitboxSystem = hitboxSystem;
        this.entityMapping = entityMapping;
    }

    public int SpawnLoadout(FactionId ownerFactionId, LoadoutPrototype prototype) {
        var nextId = ++idCounter;
        var model = new LoadoutModel(nextId);
        registry[nextId] = model;

        model.HostFactionId = ownerFactionId;
        model.Position = prototype.position;
        model.HealthId = healthSystem.Add(prototype.healthPrototype);
        model.HitboxId = hitboxSystem.Add(ownerFactionId, prototype.position, prototype.hitboxPrototype);
        model.WeaponId = weaponController.SpawnWeapon(ownerFactionId, prototype.weaponPrototype);
        model.WeaponLocalOffset = prototype.weaponOffset;

        entityMapping.CreateMapping(model.HitboxId, new EntityComponents {
            healthId = model.HealthId,
            factionId = ownerFactionId
        });

        view.AddLoadout(model.Id, prototype.position, prototype.visualsPrefab);
        return model.Id;
    }

    public void MoveLoadout(int loadoutId, Vector3 position, Quaternion rotation) {
        var model = registry[loadoutId];
        model.Position = position;
        view.UpdateTransforms(loadoutId, position, rotation);
        hitboxSystem.UpdatePosition(model.HitboxId, position);
        weaponController.MoveWeapon(model.WeaponId, position + model.WeaponLocalOffset);
    }

    public void DeleteLoadout(int loadoutId) {
        registry.Remove(loadoutId, out var model);
        hitboxSystem.Remove(model.HitboxId);
        healthSystem.Remove(model.HealthId);
        weaponController.DeleteWeapon(model.WeaponId);
        entityMapping.DeleteMapping(model.HitboxId);
        view.RemoveLoadout(loadoutId);
    }

    public LoadoutState ReadLoadoutState(int loadoutId) {
        var model = registry[loadoutId];
        return new LoadoutState {
            weaponId = model.WeaponId,
            weaponState = weaponController.ReadWeaponState(model.WeaponId),
            healthId = model.HealthId,
            healthState = healthSystem.ReadState(model.HealthId),
            factionState = factionRegistry.ReadState(model.HostFactionId)
        };
    }

    public void Update() {
        foreach (var model in registry.Values) {
            var healthState = healthSystem.ReadState(model.HealthId);
            if (healthState.damageResult.HasValue)
                view.ShowTakeHit(model.Id);
        }
    }
}

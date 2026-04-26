using System;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutController {

    private readonly LoadoutView view;
    private readonly WeaponController weaponController;

    private int idCounter;
    private readonly Dictionary<int, LoadoutModel> registry = new ();

    public LoadoutController(LoadoutView view, WeaponController weaponController) {
        this.view = view;
        this.weaponController = weaponController;
    }

    public int SpawnLoadout(int ownerCombatId, LoadoutPrototype prototype) {
        var nextId = ++idCounter;
        var model = new LoadoutModel(nextId, prototype.config);
        registry[nextId] = model;

        model.WeaponId = weaponController.SpawnWeapon(ownerCombatId, prototype.localWeaponPrototype);
        model.WeaponLocalOffset = prototype.localWeaponPrototype.position;

        view.AddLoadout(model.Id, prototype.position, prototype.shellVisualsPrefab);
        return model.Id;
    }

    public void MoveLoadout(int loadoutId, Vector3 position, Quaternion rotation) {
        var model = registry[loadoutId];
        model.Position = position;
        view.UpdateTransforms(loadoutId, position, rotation);
        weaponController.MoveWeapon(model.WeaponId, position + model.WeaponLocalOffset);
    }

    public void DeleteLoadout(int loadoutId) {
        var model = registry[loadoutId];
        weaponController.DeleteWeapon(model.WeaponId);
        view.RemoveLoadout(loadoutId);
        registry.Remove(loadoutId);
    }

    public LoadoutState ReadLoadoutState(int loadoutId) {
        var model = registry[loadoutId];
        return new LoadoutState {
            weaponId = model.WeaponId,
            weaponState = weaponController.ReadWeaponState(model.WeaponId)
        };
    }

    public void Update() {
        // should move weapon here probably
    }
}

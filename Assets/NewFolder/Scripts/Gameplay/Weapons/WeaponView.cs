using System;
using System.Collections.Generic;

using UnityEngine;

public class WeaponView {

    private Dictionary<int, WeaponVisuals> visualsRegistry = new ();

    internal void AddWeapon(int weaponId, Vector3 position, WeaponVisuals visualsPrefab) {
        var visuals = GameObject.Instantiate(visualsPrefab, position, Quaternion.identity);
        visualsRegistry[weaponId] = visuals;
    }

    internal void UpdatePosition(int weaponId, Vector3 position) {
        var visuals = visualsRegistry[weaponId];
        visuals.UpdatePosition(position);
    }

    internal void UpdateAim(int weaponId, Vector3 aimPoint, BallisticPrototype ballisticPrototype) {
        var visuals = visualsRegistry[weaponId];
        visuals.UpdateAimPoint(aimPoint, ballisticPrototype);
    }

    internal void ShowActivation(int weaponId, BallisticPrototype ballisticPrototype) {
        var visuals = visualsRegistry[weaponId];
        visuals.ShowActivation(ballisticPrototype.type);
    }

    internal void RemoveWeapon(int weaponId) {
        var visuals = visualsRegistry[weaponId];
        GameObject.Destroy(visuals.gameObject);
        visualsRegistry.Remove(weaponId);
    }
}
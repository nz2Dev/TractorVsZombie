using System;
using System.Collections.Generic;

using UnityEngine;

public class WeaponView {

    private Dictionary<int, WeaponVisuals> visualsRegistry = new ();
    private Dictionary<int, WeaponOutline> outlinesRegistry = new ();

    internal void AddWeapon(int weaponId, Vector3 position, WeaponVisuals visualsPrefab, WeaponOutline outlinePrefab, bool factionIsAlie) {
        var visuals = GameObject.Instantiate(visualsPrefab, position, Quaternion.identity);
        visuals.SetFactionAccent(factionIsAlie ? MaterialStyle.FactionAlie : MaterialStyle.FactionFoe);
        visualsRegistry[weaponId] = visuals;
        
        var outline = GameObject.Instantiate(outlinePrefab, position, Quaternion.identity);
        outlinesRegistry[weaponId] = outline;
    }

    internal void UpdatePosition(int weaponId, Vector3 position) {
        var visuals = visualsRegistry[weaponId];
        visuals.UpdatePosition(position);
        
        var outline = outlinesRegistry[weaponId];
        outline.Position = position;
    }

    internal void UpdateAim(int weaponId, Vector3 aimPoint, BallisticPrototype ballisticPrototype) {
        var visuals = visualsRegistry[weaponId];
        visuals.UpdateAimPoint(aimPoint, ballisticPrototype);

        var outline = outlinesRegistry[weaponId];
        outline.AimPoint = aimPoint;
    }

    internal void ShowActivation(int weaponId, BallisticPrototype ballisticPrototype) {
        var visuals = visualsRegistry[weaponId];
        visuals.ShowActivation(ballisticPrototype.type);
    }

    internal void RemoveWeapon(int weaponId) {
        var visuals = visualsRegistry[weaponId];
        GameObject.Destroy(visuals.gameObject);
        visualsRegistry.Remove(weaponId);

        outlinesRegistry.Remove(weaponId, out var outline);
        outline.DestroySelf();
    }
}
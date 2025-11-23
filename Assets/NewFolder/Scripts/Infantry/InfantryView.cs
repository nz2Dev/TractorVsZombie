using System.Collections.Generic;

using UnityEngine;

public class InfantryView {
    
    private readonly Dictionary<int, InfantryVisuals> visualsRegistry = new Dictionary<int, InfantryVisuals>();

    public void AddVisuals(int infantryId, Vector3 position, InfantryVisuals visualsPrefab) {
        var visuals = GameObject.Instantiate(visualsPrefab, position, Quaternion.identity);
        visualsRegistry.Add(infantryId, visuals);
    }

    public void UpdateTransform(int infantryId, Vector3 position, Quaternion rotation) {
        visualsRegistry[infantryId].UpdatePositionAndRotation(position, rotation);
    }

    internal void ShowTakeHit(int infantryId) {
        visualsRegistry[infantryId].PlayTakeHit();
    }

    public void ShowDirectFrontAttack(int infantryId) {
        visualsRegistry[infantryId].PlayDirectAttackAnimation();
    }

    public void ShowDeathByProjectile(int infantryId, Vector3 damageSourcePosition, bool blownAway) {
        var visuals = visualsRegistry[infantryId];
        if (blownAway) {
            visuals.RotateAway(damageSourcePosition);
            visuals.PlayPushedAwayDeathAnimation();
        } else {
            visuals.PlayDisolveAnimation();
        }
    }

    internal void ShowDisolveDeath(int infantryId) {
        var visuals = visualsRegistry[infantryId];
        visuals.PlayDisolveAnimation();
    }

    public void RemoveVisuals(int infantryId) {
        var visuals = visualsRegistry[infantryId];
        visuals.DestroySelfOnIdle();
        visualsRegistry.Remove(infantryId);
    }
}

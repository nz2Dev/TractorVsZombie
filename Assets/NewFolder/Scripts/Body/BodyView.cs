using System.Collections.Generic;

using UnityEngine;

public class BodyView {
    
    private readonly Dictionary<int, BodyVisuals> visualsRegistry = new Dictionary<int, BodyVisuals>();

    public void AddVisuals(int infantryId, Vector3 position, BodyVisuals visualsPrefab) {
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

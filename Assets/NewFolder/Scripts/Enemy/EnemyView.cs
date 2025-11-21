using System.Collections.Generic;

using UnityEngine;

public class EnemyView {

    private readonly Dictionary<int, UnitVisuals> unitVisuals = new Dictionary<int, UnitVisuals>();
    private readonly UnitVisuals visualsPrefab;

    public EnemyView(UnitVisuals visualsPrefab) {
        this.visualsPrefab = visualsPrefab;
    }

    public void AddUnit(int unitId, Vector3 position) {
        var visuals = GameObject.Instantiate(visualsPrefab, position, Quaternion.identity);
        unitVisuals.Add(unitId, visuals);
    }

    public void UpdateUnitPositionAndRotation(int unitId, Vector3 position, Quaternion rotation) {
        unitVisuals[unitId].UpdatePositionAndRotation(position, rotation);
    }

    internal void ShowTakeHit(int unitId) {
        unitVisuals[unitId].PlayTakeHit();
    }

    public void ShowDirectFrontAttack(int unitId) {
        unitVisuals[unitId].PlayDirectAttackAnimation();
    }

    public void ShowDeathByProjectile(int unitId, Vector3 damageSourcePosition, bool blownAway) {
        var visuals = unitVisuals[unitId];
        if (blownAway) {
            visuals.RotateAway(damageSourcePosition);
            visuals.PlayPushedAwayDeathAnimation();
        } else {
            visuals.PlayDisolveAnimation();
        }
    }

    internal void ShowDisolveDeath(int unitId) {
        var visuals = unitVisuals[unitId];
        visuals.PlayDisolveAnimation();
    }

    public void RemoveUnit(int unitId) {
        var visuals = unitVisuals[unitId];
        visuals.DestroySelfOnIdle();
        unitVisuals.Remove(unitId);
    }

}
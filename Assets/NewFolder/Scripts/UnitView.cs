using System.Collections.Generic;
using System.Data.Common;

using UnityEngine;
using UnityEngine.Assertions;

public class UnitView {

    private Dictionary<int, UnitVisuals> unitVisuals = new Dictionary<int, UnitVisuals>();

    private readonly UnitVisuals visualsPrefab;

    public UnitView(UnitVisuals visualsPrefab) {
        this.visualsPrefab = visualsPrefab;
    }

    public void AddUnit(int unitId, Vector3 position) {
        var visuals = GameObject.Instantiate(visualsPrefab, position, Quaternion.identity);
        unitVisuals.Add(unitId, visuals);
    }

    public void UpdateUnitPositionAndRotation(int unitId, Vector3 position, Quaternion rotation) {
        unitVisuals[unitId].UpdatePositionAndRotation(position, rotation);
    }

    public void ShowDirectFrontAttack(int unitId) {
        unitVisuals[unitId].PlayDirectAttackAnimation();
    }

    public void ShowFinalBlow(int unitId, Vector3 damageSourcePosition) {
        var visuals = unitVisuals[unitId];
        visuals.RotateAway(damageSourcePosition);
        visuals.PlayFinalBlowAnimation();
    }

    public void RemoveUnit(int unitId) {
        var visuals = unitVisuals[unitId];
        visuals.DestroySelfOnIdle();
        unitVisuals.Remove(unitId);
    }

}
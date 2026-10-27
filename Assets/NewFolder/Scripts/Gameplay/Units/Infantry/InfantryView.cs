using System;
using System.Collections.Generic;

using UnityEngine;

public class InfantryView {
    
    private readonly Dictionary<int, InfantryVisuals> visualsRegistry = new Dictionary<int, InfantryVisuals>();

    public void AddVisuals(int infantryId, Vector3 position, InfantryVisuals visualsPrefab) {
        var visuals = GameObject.Instantiate(visualsPrefab, position, Quaternion.identity);
        visualsRegistry.Add(infantryId, visuals);
    }

    public void SetTargetPose(int infantryId, Vector3 position, Quaternion rotation) {
        var visuals = visualsRegistry[infantryId];
        visuals.SetPosition(position);
        visuals.SetTargetRotation(rotation);
    }

    public void SetCurrentPose(int infantryId, Vector3 position, Quaternion rotation) {
        var visuals = visualsRegistry[infantryId];
        visuals.SetPosition(position);
        visuals.SetRotation(rotation);
    }

    public void ShowMovementSpeed(int infantryId, float speedNormalized) {
        var visuals = visualsRegistry[infantryId];
        visuals.SetMovementSpeed(speedNormalized);
    }

    internal void ShowTakeHit(int infantryId) {
        visualsRegistry[infantryId].PlayTakeHit();
    }

    internal void SetHealthNormalized(int infantryId, float healthLoadNormalized) {
        visualsRegistry[infantryId].SetHealthLoad(healthLoadNormalized);
    }

    internal void ShowCharge(int infantryId, float duration) {
        var visuals = visualsRegistry[infantryId];
        visuals.PlayCharge(duration);
    }

    internal void ShowDischarge(int infantryId) {
        var visuals = visualsRegistry[infantryId];
        visuals.PlayDischarge();
    }

    public void PlayChargeAttack(int infantryId) {
        var visuals = visualsRegistry[infantryId];
        visuals.PlayChargeAttack();
    }

    public void ShowThrownAway(int infantryId) {
        var visuals = visualsRegistry[infantryId];
        visuals.PlayPushedAwayDeathAnimation();
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

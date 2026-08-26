using System.Collections.Generic;

using UnityEngine;

public class InfantryView {
    
    private readonly FootstepSoundSystem footstepSound;
    private readonly Dictionary<int, InfantryVisuals> visualsRegistry = new Dictionary<int, InfantryVisuals>();

    public InfantryView(FootstepSoundSystem footstepSound) {
        this.footstepSound = footstepSound;
    }

    public void AddVisuals(int infantryId, Vector3 position, InfantryVisuals visualsPrefab) {
        var visuals = GameObject.Instantiate(visualsPrefab, position, Quaternion.identity);
        visualsRegistry.Add(infantryId, visuals);
    }

    public void UpdateTransform(int infantryId, Vector3 position, Quaternion rotation, float speedNormalized) {
        var visuals = visualsRegistry[infantryId];
        visuals.UpdatePositionAndRotation(position, rotation);
        visuals.SetSpeed(speedNormalized);
        footstepSound.RegisterFootstep(position, speedNormalized);
    }

    internal void ShowTakeHit(int infantryId) {
        visualsRegistry[infantryId].PlayTakeHit();
    }

    public void ShowDirectFrontAttack(int infantryId, Vector3 targetPosition) {
        var visuals = visualsRegistry[infantryId];
        visuals.PlayDirectAttackAnimation();
    }

    public void ShowThrownAway(int infantryId, Vector3 sourcePosition) {
        var visuals = visualsRegistry[infantryId];
        var sourceToPosition = visuals.transform.position - sourcePosition; sourceToPosition.y = 0;
        visuals.SetOverrideRotation(Quaternion.LookRotation(-sourceToPosition.normalized, Vector3.up));
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

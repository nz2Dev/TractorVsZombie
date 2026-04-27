using System.Collections.Generic;

using UnityEngine;

public class RocketView {
    
    private readonly SoundManager soundManager;
    private readonly Dictionary<int, RocketVisuals> visualsRegistry = new ();

    public RocketView(SoundManager soundManager) {
        this.soundManager = soundManager;
    }

    internal void ShowRocketFly(int rocketId, RocketVisuals visualsPrefab, 
        float startTime, float duration, 
        FlyPath trajectory, FlyShape flyShape, AudioClip[] launchEffectClips) {
        var visuals = GameObject.Instantiate(visualsPrefab, trajectory.launchPoint, Quaternion.identity);
        visualsRegistry[rocketId] = visuals;
        visuals.Setup(trajectory.launchPoint, trajectory.landPoint, 
            startTime, duration, flyShape.amplitude, flyShape.curve);
        soundManager.PlayEffect(trajectory.launchPoint, launchEffectClips);
    }

    internal void ShowRocketExplosion(int rocketId, Vector3 landPoint, AudioClip[] explosionSFX) {
        soundManager.PlayEffect(landPoint, explosionSFX);
        var visuals = visualsRegistry[rocketId];
        GameObject.Destroy(visuals.gameObject);
        visualsRegistry.Remove(rocketId);
    }
}
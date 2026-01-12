using System.Collections.Generic;

using UnityEngine;

public class RocketView {
    
    private Dictionary<int, RocketVisuals> visualsRegistry = new ();

    internal void ShowRocketFly(int rocketId, float startTime, RocketTrajectory trajectory, RocketConfig config) {
        var visuals = GameObject.Instantiate(config.visualsPrefab, trajectory.launchPoint, Quaternion.identity);
        visuals.Setup(trajectory.launchPoint, trajectory.landPoint, startTime, config.flyDuration, config.amplitude, config.flyCurve);
        visualsRegistry[rocketId] = visuals;
    }

    internal void ShowRocketExplosion(int rocketId) {
        var visuals = visualsRegistry[rocketId];
        GameObject.Destroy(visuals.gameObject);
        visualsRegistry.Remove(rocketId);
    }
}
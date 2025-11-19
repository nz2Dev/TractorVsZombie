using System.Collections.Generic;

using UnityEngine;

public class RocketView : MonoBehaviour {
    
    [SerializeField] private RocketVisuals rocketVisualsPrefab;

    private Dictionary<int, RocketVisuals> visualsRegistry;

    private void Awake() {
        visualsRegistry = new ();
    }

    internal void ShowRocketFly(int rocketId, float startTime, RocketTrajectory trajectory) {
        var visuals = GameObject.Instantiate(rocketVisualsPrefab, trajectory.launchPoint, Quaternion.identity);
        visuals.Setup(trajectory.launchPoint, trajectory.landPoint, startTime, trajectory.flyDuration);
        visualsRegistry[rocketId] = visuals;
    }

    internal void ShowRocketExplosion(int rocketId) {
        var visuals = visualsRegistry[rocketId];
        GameObject.Destroy(visuals.gameObject);
        visualsRegistry.Remove(rocketId);
    }
}
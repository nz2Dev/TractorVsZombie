using System;
using UnityEngine;
using UnityEngine.Assertions;

public static class PathSteeringAlgorithm {
    public static Vector3 FollowPath(this Vehicle vehicle, Path path) {
        Assert.IsNotNull(path);
        var futurePosition = vehicle.PredictPosition(1);
        var pointOnPath = path.FindClosestNormalPoint(futurePosition);
        return vehicle.Seek(pointOnPath);
    }
}

public class PathSteering : MonoBehaviour, ISteering {
    [SerializeField] private Path path;
    [SerializeField] private float weight = 1f;

    public Color Color => Color.white;
    public string Source => "Path";
    public float Weight => weight;

    public Vector3 CalculateSteeringForce(Vehicle vehicle) {
        return vehicle.FollowPath(path);
    }

    public void DrawGizmosSelected(Vehicle vehicle) {
    }
}
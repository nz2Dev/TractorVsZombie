using UnityEngine;

public static class PathSteeringAlgorithm {
    public static Vector3 FollowPath(this Vehicle vehicle, Path path) {
        if (path == null) {
            return default;
        }

        var futurePosition = vehicle.PredictPosition(1);
        if (path.FindClosestNormalPoint(futurePosition, out var pointOnPath)) {
            return vehicle.Seek(pointOnPath);
        }

        return default;
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

    public void OnDrawGizmosSelected(Vehicle vehicle) {
    }
}
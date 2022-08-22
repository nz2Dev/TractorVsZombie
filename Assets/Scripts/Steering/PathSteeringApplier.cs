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

public class PathSteeringApplier : MonoBehaviour {
    [SerializeField] private Path path;
    [SerializeField] private float weight = 1f;

    private Vehicle _vehicle;

    private void Awake() {
        _vehicle = GetComponent<Vehicle>();
    }

    private void Update() {
        var steeringForce = _vehicle.FollowPath(path);
        if (steeringForce != default) {
            _vehicle.ApplyForce(steeringForce * weight);
        }
    }
}
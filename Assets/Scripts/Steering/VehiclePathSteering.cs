using UnityEngine;

public class VehiclePathSteering : MonoBehaviour {
    [SerializeField] private Path followingPath;
    [SerializeField][Range(0, 3f)] private float pathFollowingSteeringWeight = 1f;

    private Vehicle _vehicle;

    private void Awake() {
        _vehicle = GetComponent<Vehicle>();
    }

    private void Update() {
        if (CalculatePathFollowingSteeringForce(followingPath, out var steeringForce)) {
            //Debug.DrawLine(_vehicle.transform.position, _vehicle.transform.position + steeringForce, Color.red, 0.1f);
            _vehicle.ApplyForce(steeringForce * pathFollowingSteeringWeight);
        }
    }

    private bool CalculatePathFollowingSteeringForce(Path path, out Vector3 steeringForce) {
        if (path == null) {
            steeringForce = default;
            return false;
        }

        var futurePosition = _vehicle.PredictPosition(1);
        if (path.FindClosestNormalPoint(futurePosition, out var pointOnPath)) {
            steeringForce = _vehicle.CalculateSeekSteeringForce(pointOnPath);
            return true;
        }

        steeringForce = default;
        return false;
    }
}
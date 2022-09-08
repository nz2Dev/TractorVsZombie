using UnityEditor;
using UnityEngine;

public static class WallStopAlgorithm {
    public static Vector3 StopOnFirstWall(this Vehicle vehicle, float distance, LayerMask layerMask) {
        if (Physics.Raycast(vehicle.Position, vehicle.Forward, out var hitInfo, distance, layerMask)) {
            return vehicle.Arrival(hitInfo.point, distance);
        } else {
            return default;
        }
    }

    public static bool TryStopOnFirstWall(this Vehicle vehicle, float distance, LayerMask layerMask, out Vector3 force) {
        force = StopOnFirstWall(vehicle, distance, layerMask);
        return force != default;
    }
}

public class WallsStopSteering : MonoBehaviour, ISteering {
    [SerializeField] private float checkDistance = 1.5f;
    [SerializeField] private LayerMask wallsLayerMask;
    [SerializeField] private float weight = 1f;

    public Color Color => Color.gray;
    public string Source => "WallsStop";
    public float Weight => weight;

    public Vector3 CalculateSteeringForce(Vehicle vehicle) {
        return vehicle.StopOnFirstWall(checkDistance, wallsLayerMask);
    }

    public void OnDrawGizmosSelected(Vehicle vehicle) {
        Handles.color = Color.gray;
        Handles.DrawWireDisc(vehicle.Position, Vector3.up, checkDistance);
    }
}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class WallAvoideAlgorithm {
    public static Vector3 AvoidWall(this Vehicle vehicle, Collider wall) {
        var awayVector = (vehicle.Position - wall.ClosestPoint(vehicle.Position));
        awayVector = Vector3.ProjectOnPlane(awayVector, Vector3.up).normalized;
        Vector3 flatForward = Vector3.ProjectOnPlane(vehicle.Forward, Vector3.up);
        var dotForwardToNormal = Vector3.Dot(awayVector, flatForward);
        var desiredVelocity = flatForward + awayVector * 2 * Mathf.Abs(dotForwardToNormal);
        return desiredVelocity - vehicle.Velocity;
    }

    public static Vector3 AvoidWallsAround(this Vehicle vehicle, float radius, LayerMask layerMask) {
        var walls = Physics.OverlapSphere(vehicle.transform.position, radius, layerMask);
        if (walls.Length == 0) {
            return default;
        }

        var accumulatedForce = Vector3.zero;
        foreach (var wall in walls) {
            var avoidForce = vehicle.AvoidWall(wall);
            accumulatedForce += avoidForce;
        }

        return accumulatedForce / walls.Length;
    }

    public static bool TryAvoidWallsAround(this Vehicle vehicle, float radius, LayerMask layerMask, out Vector3 force) {
        force = AvoidWallsAround(vehicle, radius, layerMask);
        return force != default;
    }
}

[ExecuteInEditMode]
[RequireComponent(typeof(Vehicle))]
public class WallsAvoideSteering : MonoBehaviour, ISteering {

    [SerializeField] private float checkRadius = 3;
    [SerializeField] private LayerMask wallsLayerMask;
    [SerializeField] private float weight = 1f;

    public Color Color => Color.yellow;
    public string Source => "WallsAvoidance";
    public float Weight => weight;

    public Vector3 CalculateSteeringForce(Vehicle vehicle) {
        return vehicle.AvoidWallsAround(checkRadius, wallsLayerMask);
    }

    public void OnDrawGizmosSelected(Vehicle vehicle) {
        Handles.DrawWireDisc(vehicle.baseTransform.position, Vector3.up, checkRadius);
    }
    
}
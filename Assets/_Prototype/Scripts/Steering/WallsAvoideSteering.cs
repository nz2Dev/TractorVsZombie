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

    public static Vector3 AvoidWallsAround(this Vehicle vehicle, float radius, LayerMask layerMask, Collider[] wallsAvoidAllocation) {
        var wallsCount = Physics.OverlapSphereNonAlloc(vehicle.transform.position, radius, wallsAvoidAllocation, layerMask);
        if (wallsCount == 0) {
            return default;
        }

        var accumulatedForce = Vector3.zero;
        for (int i = 0; i < wallsCount; i++) {
            var avoidForce = vehicle.AvoidWall(wallsAvoidAllocation[i]);
            accumulatedForce += avoidForce;
        }

        return accumulatedForce / wallsCount;
    }

    public static bool TryAvoidWallsAround(this Vehicle vehicle, float radius, LayerMask layerMask, Collider[] wallsAvoidAllocation, out Vector3 force) {
        force = AvoidWallsAround(vehicle, radius, layerMask, wallsAvoidAllocation);
        return force != default;
    }
}

[ExecuteInEditMode]
[RequireComponent(typeof(Vehicle))]
public class WallsAvoideSteering : MonoBehaviour, ISteering {

    [SerializeField] private float checkRadius = 3;
    [SerializeField] private LayerMask wallsLayerMask;
    [SerializeField] private float weight = 1f;

    private Collider[] _allocation = new Collider[5];

    public Color Color => Color.yellow;
    public string Source => "WallsAvoidance";
    public float Weight => weight;

    public Vector3 CalculateSteeringForce(Vehicle vehicle) {
        return vehicle.AvoidWallsAround(checkRadius, wallsLayerMask, _allocation);
    }

#if UNITY_EDITOR
    public void DrawGizmosSelected(Vehicle vehicle) {
        Handles.DrawWireDisc(vehicle.Position, Vector3.up, checkRadius);
    }
#endif

}
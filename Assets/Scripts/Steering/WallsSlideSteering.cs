using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public static class WallSlideAlgorithm {
    public static Vector3 SlideWall(this Vehicle vehicle, Collider wall, float offset) {
        var vehicleTransform = vehicle.transform;
        var position = vehicleTransform.position;

        var closestPointOnWall = wall.ClosestPoint(position);
        var wallToVehicleVector = position - closestPointOnWall;
        // Debug.DrawRay(wall.ClosestPoint(position), wallToVehicleVector, Color.blue, 0.1f);
        var wallToVehicleNormal = wallToVehicleVector.normalized;
        var flatForward = Vector3.ProjectOnPlane(vehicleTransform.forward, Vector3.up);
        var forwardOnWallProjection = Vector3.ProjectOnPlane(flatForward, wallToVehicleNormal);
        // Debug.Log("projection: " + forwardOnWallProjection + " magnitued: " + forwardOnWallProjection.magnitude + " normalized: " + forwardOnWallProjection.normalized);

        var slideDirection = (Vector3)default;
        if (forwardOnWallProjection.magnitude > Vector3.kEpsilon) {
            slideDirection = forwardOnWallProjection.normalized;
        } else {
            slideDirection = Vector3.ProjectOnPlane(vehicleTransform.right, Vector3.up);
        }
        // Debug.DrawRay(position, slideDirection, Color.white, 0.1f);

        var seekPosition = closestPointOnWall + wallToVehicleNormal * offset + slideDirection;
        //Debug.DrawLine(position, seekPosition);

        return vehicle.Seek(seekPosition);
    }

    public static Vector3 SlideWallsAround(this Vehicle vehicle, float radius, LayerMask layerMask) {
        var walls = Physics.OverlapSphere(vehicle.transform.position, radius, layerMask);
        if (walls.Length == 0) {
            return default;
        }

        var accumulatedForce = Vector3.zero;
        foreach (var wall in walls) {
            var slideForce = vehicle.SlideWall(wall, radius);
            accumulatedForce += slideForce;
        }

        return accumulatedForce / walls.Length;
    }

    public static bool TrySlideWallsAround(this Vehicle vehicle, float radius, LayerMask layerMask, out Vector3 force) {
        force = SlideWallsAround(vehicle, radius, layerMask);
        return force != default;
    }
}

[ExecuteInEditMode]
public class WallsSlideSteering : MonoBehaviour, ISteering {
    
    [SerializeField] private float checkRadius = 1f;
    [SerializeField] private LayerMask wallsLayerMask;
    [SerializeField] private float weight = 1f;

    public Color Color => Color.magenta;
    public string Source => "Wall Sliding";
    public float Weight => weight;

    public Vector3 CalculateSteeringForce(Vehicle vehicle) {
        return vehicle.SlideWallsAround(checkRadius, wallsLayerMask);
    }

    public void OnDrawGizmosSelected(Vehicle vehicle) {
        Handles.color = Color.magenta;
        Handles.DrawWireDisc(vehicle.baseTransform.position, Vector3.up, checkRadius);
    }
}
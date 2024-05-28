using System.Linq;
using UnityEditor;
using UnityEngine;

public static class SeparationSteerAlgorithm {
    public static Vector3 Separate(this Vehicle vehicle, Collider[] neighbors, int neighborsCount) {
        if (neighborsCount == 0) {
            return default;
        }

        var accumulativeFleeForce = Vector3.zero;
        var vehiclePosition = vehicle.transform.position;
        //var maxDistance = 3;

        int fleeCount = 0;
        for (int i = 0; i < neighborsCount; i++) {
            if (neighbors[i].attachedRigidbody != null) {
                var neighborFlee = vehicle.Flee(neighbors[i].attachedRigidbody.transform.position);
                //var distance = Vector3.Distance(neighbor.transform.position, vehiclePosition);
                //var distanceClamped = Mathf.Max(distance, maxDistance);
                //neighborFlee *= (distanceClamped / maxDistance);
                accumulativeFleeForce += neighborFlee;
                fleeCount++;
            }
        }

        accumulativeFleeForce /= fleeCount;
        return accumulativeFleeForce - vehicle.Velocity;
    }

    public static Vector3 SeparateInsideSphere(this Vehicle vehicle, float radius, LayerMask layerMask, Collider[] allocationArray) {
        var position = vehicle.transform.position;
        int neighborsCount = Physics.OverlapSphereNonAlloc(position, 1/* radius */, allocationArray, layerMask);
        return vehicle.Separate(allocationArray, neighborsCount);
        // var neighbors = Physics.OverlapSphere(position, radius, layerMask);
        // return vehicle.Separate(neighbors, neighbors.Length);
    }

    public static bool TrySeparateInsideSphere(this Vehicle vehicle, float radius, LayerMask layerMask, Collider[] allocationArray, out Vector3 force) {
        force = SeparateInsideSphere(vehicle, radius, layerMask, allocationArray);
        return force != default;
    }
}

public class SeparationSteering : MonoBehaviour, ISteering {
    [SerializeField] private float checkRadius = 5f;
    [SerializeField] private LayerMask neighborsLayerMask;
    [SerializeField] private float weight = 1f;

    private Collider[] allocations = new Collider[10];

    public Color Color => Color.magenta;
    public string Source => "Separation";
    public float Weight => weight;

    public Vector3 CalculateSteeringForce(Vehicle vehicle) {
        return vehicle.SeparateInsideSphere(checkRadius, neighborsLayerMask, allocations);
    }

#if UNITY_EDITOR
    public void DrawGizmosSelected(Vehicle vehicle) {
        Handles.color = Color.red;
        Handles.DrawSolidDisc(vehicle.transform.position, Vector3.up, checkRadius);
    }
#endif
}
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public static class SeparationSteerAlgorithm {
    public static Vector3 Separate(this Vehicle vehicle, GameObject[] neighbors) {
        if (neighbors.Length == 0) {
            return default;
        }

        var accumulativeFleeForce = Vector3.zero;
        var vehiclePosition = vehicle.transform.position;
        //var maxDistance = 3;

        foreach (var neighbor in neighbors) {
            var neighborFlee = vehicle.Flee(neighbor.transform.position);
            //var distance = Vector3.Distance(neighbor.transform.position, vehiclePosition);
            //var distanceClamped = Mathf.Max(distance, maxDistance);
            //neighborFlee *= (distanceClamped / maxDistance);
            accumulativeFleeForce += neighborFlee;
        }

        accumulativeFleeForce /= neighbors.Length;
        return accumulativeFleeForce - vehicle.Velocity;
    }

    public static Vector3 SeparateInsideSphere(this Vehicle vehicle, float radius, LayerMask layerMask) {
        var position = vehicle.transform.position;

        var colliders = Physics.OverlapSphere(position, radius, layerMask);
        var neighbors = colliders.Select(c => c.attachedRigidbody == null ? null : c.attachedRigidbody.gameObject)
            .Where(it => it != null)
            .Distinct()
            .ToArray();

        return vehicle.Separate(neighbors);
    }

    public static bool TrySeparateInsideSphere(this Vehicle vehicle, float radius, LayerMask layerMask, out Vector3 force) {
        force = SeparateInsideSphere(vehicle, radius, layerMask);
        return force != default;
    }
}

public class SeparationSteering : MonoBehaviour, ISteering {
    [SerializeField] private float checkRadius = 5f;
    [SerializeField] private LayerMask neighborsLayerMask;
    [SerializeField] private float weight = 1f;

    public Color Color => Color.magenta;
    public string Source => "Separation";
    public float Weight => weight;

    public Vector3 CalculateSteeringForce(Vehicle vehicle) {
        return vehicle.SeparateInsideSphere(checkRadius, neighborsLayerMask);
    }

#if UNITY_EDITOR
    public void OnDrawGizmosSelected(Vehicle vehicle) {
        Handles.color = Color.red;
        Handles.DrawSolidDisc(vehicle.transform.position, Vector3.up, checkRadius);
    }
#endif
}
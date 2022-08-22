using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class VehicleSeparationSteering : MonoBehaviour {
    [SerializeField] private LayerMask vehicleLayerMask;
    [SerializeField][Range(0, 3f)] private float separationSteeringWeight = 1f;

    private Vehicle _vehicle;

    private void Awake() {
        _vehicle = GetComponent<Vehicle>();
    }

    private void Update() {
        var vehiclePosition = _vehicle.transform.position;
        var casted = Physics.SphereCastAll(vehiclePosition, 2, Vector3.up, 10, vehicleLayerMask);
        var neighbors = casted
            .Where(hit => hit.rigidbody != null && hit.rigidbody.gameObject != gameObject)
            .Select(hit => hit.rigidbody.gameObject)
            .ToArray();

        if (CalculateSeparationSteeringForce(neighbors, out var separationForce)) {
            _vehicle.ApplyForce(separationForce * separationSteeringWeight, "separation", Color.magenta); // * 1f
        }
    }

    private bool CalculateSeparationSteeringForce(GameObject[] neighbors, out Vector3 desireVelocity) {
        if (neighbors.Length == 0) {
            desireVelocity = default;
            return false;
        }

        var accumulativeFleeForce = Vector3.zero;
        var vehiclePosition = _vehicle.transform.position;
        //var maxDistance = 3;

        foreach (var neighbor in neighbors) {
            var neighborFlee = _vehicle.Flee(neighbor.transform.position);
            //var distance = Vector3.Distance(neighbor.transform.position, vehiclePosition);
            //var distanceClamped = Mathf.Max(distance, maxDistance);
            //neighborFlee *= (distanceClamped / maxDistance);
            accumulativeFleeForce += neighborFlee;
        }

        accumulativeFleeForce /= neighbors.Length;
        desireVelocity = accumulativeFleeForce - _vehicle.Velocity;
        return true;
    }
}
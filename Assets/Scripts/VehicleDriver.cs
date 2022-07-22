using System;
using System.Linq;
using UnityEngine;

public class VehicleDriver : MonoBehaviour {
    
    [SerializeField] private GameObject target;
    [SerializeField] private Path followingPath;
    [SerializeField] private float slowingDistance = 1.5f;
    [SerializeField] private LayerMask vehicleLayerMask;
    
    [SerializeField] [Range(0, 3f)] private float arrivalSteeringWeight = 1f;
    [SerializeField] [Range(0, 3f)] private float separationSteeringWeight = 1f;
    [SerializeField] [Range(0, 3f)] private float pathFollowingSteeringWeight = 1f;
    
    private Vehicle _vehicle;

    public GameObject Target => target;

    private void Awake() {
        _vehicle = GetComponent<Vehicle>();
        SetTarget(target);
    }

    public void Stop() {
        _vehicle.enabled = false;
    }

    public void Resume() {
        _vehicle.enabled = true;
    }

    public void SetTarget(GameObject newTarget) {
        _vehicle.enabled = newTarget != null;
        target = newTarget;
    }

    private void Update() {
        if (target == null) {
            return;
        }

        var vehiclePosition = _vehicle.transform.position;
        
        // var clampedDistance = Mathf.Clamp(distance, 0, 5);
        // var separationWeightRemapped = Utils.Remap(5 - clampedDistance, 0, 5, 1, separationSteeringWeight);
        // var arrivalSteeringWeightRemapped = Utils.Remap(clampedDistance, 0, 5, 1.5f, arrivalSteeringWeight);
        
        var targetPosition = target.transform.position;
        var arrivalForce = CalculateArrivalSteeringForce(targetPosition);
        //Debug.DrawLine(vehiclePosition, vehiclePosition + arrivalForce, Color.red);
        //Debug.DrawLine(_vehicle.transform.position, _vehicle.transform.position + arrivalForce, Color.blue, 0.1f);
        _vehicle.ApplyForce(arrivalForce * arrivalSteeringWeight, "Arrival", Color.blue); // * 1.5f
        
        var casted = Physics.SphereCastAll(vehiclePosition, 2, Vector3.up, 10, vehicleLayerMask);
        var neighbors = casted
            .Where(hit => hit.rigidbody != null && hit.rigidbody.gameObject != gameObject)
            .Select(hit => hit.rigidbody.gameObject)
            .ToArray();
        
        if (CalculateSeparationSteeringForce(neighbors, out var separationForce)) {
            //Debug.DrawLine(vehiclePosition, vehiclePosition + separationForce, Color.blue);
            _vehicle.ApplyForce(separationForce * separationSteeringWeight); // * 1f
        }

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
            steeringForce = CalculateSeekSteeringForce(pointOnPath);
            return true;    
        }

        steeringForce = default;
        return false;
    }
    
    private bool CalculateSeparationSteeringForce(GameObject[] neighbors, out Vector3 desireVelocity) {
        if (neighbors.Length == 0) {
            desireVelocity = default;
            return false;
        }
        
        var accumulativeFleeForce = Vector3.zero;
        var vehiclePosition = _vehicle.transform.position;
        var maxDistance = 3;
        
        foreach (var neighbor in neighbors) {
            var neighborFlee = CalculateFleeSteeringForce(neighbor.transform.position);
            //var distance = Vector3.Distance(neighbor.transform.position, vehiclePosition);
            //var distanceClamped = Mathf.Max(distance, maxDistance);
            //neighborFlee *= (distanceClamped / maxDistance);
            accumulativeFleeForce += neighborFlee;
        }

        accumulativeFleeForce /= neighbors.Length;
        desireVelocity = accumulativeFleeForce - _vehicle.Velocity;
        return true;
    }
    
    private Vector3 CalculateArrivalSteeringForce(Vector3 targetPosition) {
        var vehiclePosition = _vehicle.transform.position;
        var distance = Vector3.Distance(targetPosition, vehiclePosition);
        var rampedSpeed = _vehicle.MaxSpeed * (distance / slowingDistance);
        var clippedSpeed = Mathf.Min(rampedSpeed, _vehicle.MaxSpeed);
        var desiredVelocity = (clippedSpeed / distance) * (targetPosition - vehiclePosition);
        return desiredVelocity - _vehicle.Velocity;
    }

    private Vector3 CalculateSeekSteeringForce(Vector3 targetPosition) {
        var desiredVelocity = (targetPosition - transform.position).normalized * _vehicle.MaxSpeed;
        return desiredVelocity - _vehicle.Velocity;
    }
    
    private Vector3 CalculateFleeSteeringForce(Vector3 targetPosition) {
        return CalculateSeekSteeringForce(targetPosition) * -1;
    }
}
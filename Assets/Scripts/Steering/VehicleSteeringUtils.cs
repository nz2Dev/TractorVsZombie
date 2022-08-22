using UnityEngine;

public static class VehicleSteeringUtils {

    public static Vector3 CalculateSeekSteeringForce(this Vehicle vehicle, Vector3 targetPosition) {
        var desiredVelocity = (targetPosition - vehicle.transform.position).normalized * vehicle.MaxSpeed;
        return desiredVelocity - vehicle.Velocity;
    }

    public static Vector3 CalculateFleeSteeringForce(this Vehicle vehicle, Vector3 targetPosition) {
        return CalculateSeekSteeringForce(vehicle, targetPosition) * -1;
    }

    public static Vector3 CalculateArrivalSteeringForce(this Vehicle vehicle, Vector3 targetPosition, float slowingDistance) {
        var vehiclePosition = vehicle.transform.position;
        var distance = Vector3.Distance(targetPosition, vehiclePosition);
        var rampedSpeed = vehicle.MaxSpeed * (distance / slowingDistance);
        var clippedSpeed = Mathf.Min(rampedSpeed, vehicle.MaxSpeed);
        var desiredVelocity = (clippedSpeed / distance) * (targetPosition - vehiclePosition);
        return desiredVelocity - vehicle.Velocity;
    }

}
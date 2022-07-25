using UnityEngine;

public static class VehicleSteeringUtils {

    public static Vector3 CalculateSeekSteeringForce(this Vehicle vehicle, Vector3 targetPosition) {
        var desiredVelocity = (targetPosition - vehicle.transform.position).normalized * vehicle.MaxSpeed;
        return desiredVelocity - vehicle.Velocity;
    }

    public static Vector3 CalculateFleeSteeringForce(this Vehicle vehicle, Vector3 targetPosition) {
        return CalculateSeekSteeringForce(vehicle, targetPosition) * -1;
    }

}
using UnityEngine;

public static class VehicleDriving {
    
    public static float GasThrottle(float gasInput, bool boostInput, float maxTorque) {
        var powerMultiplier = boostInput ? 2 : 1;
        return gasInput * powerMultiplier * maxTorque;
    }

    public static float LimitSteering(float vehicleSpeed, VehicleDrivingConfig config) {
        float t = Mathf.Clamp01(vehicleSpeed / config.speedCeilingForSteering);
        float steerFactor = 1f - Mathf.Pow(t, config.speedKFactor); // k > 1 makes the falloff sharper near top speed
        return Mathf.Max(config.minStterAmount, steerFactor);
    }

    public static float SteerToward(Vector3 directionInput, Vector3 vehicleVelocity, float vehicleMaxSteerDegrees) {
        var forward = vehicleVelocity;
        var forwardToDirectionDegrees = Vector3.SignedAngle(forward, directionInput, Vector3.up);
        return Mathf.Clamp(forwardToDirectionDegrees, -vehicleMaxSteerDegrees, vehicleMaxSteerDegrees);
    }

}
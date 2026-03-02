using UnityEngine;

public static class Steering {

    public const float DefaultCohesionWeight = 0.4f;
    public const float DefaultSpeedAdjustFactor = 0.4f;

    public static Vector3 CohesionSteering(Vector3 position, Vector3 direction, float maxSpeed, CohesionInput cohesionInput) {
        var cohesionForce = (cohesionInput.center - position).normalized;

        var blend = Vector3.Lerp(direction, cohesionForce, DefaultCohesionWeight).normalized;

        float relativePosition = Vector3.Dot(position - cohesionInput.center, cohesionInput.direction);
        var speedFactor = relativePosition > 0 ? Mathf.Clamp01(1f - relativePosition * DefaultSpeedAdjustFactor) : 1f;

        return maxSpeed * speedFactor * blend;
    }
}

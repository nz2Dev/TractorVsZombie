using UnityEngine;

public static class Steering {

    public const float DefaultSpeedAdjustFactor = 0.4f;

    public static Vector3 Cohese(Vector3 position, float maxSpeed, Vector3 center, Vector3 direction) {
        var cohesionDirection = (center - position).normalized;

        float relativePosition = Vector3.Dot(position - center, direction);
        var speedFactor = relativePosition > 0 ? Mathf.Clamp01(1f - relativePosition * DefaultSpeedAdjustFactor) : 1f;

        return maxSpeed * speedFactor * cohesionDirection;
    }
}

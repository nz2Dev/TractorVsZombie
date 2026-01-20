using UnityEngine;

public static class Steering {

    public const float DefaultCohesionWeight = 0.3f;
    public const float DefaultSpeedAdjustFactor = 0.2f;
    
    public static Vector3 ComputeCohesionForce(Vector3 position, Vector3 center, float weight) {
        if (weight <= 0) 
            return Vector3.zero;
        
        Vector3 toCenter = center - position;
        return toCenter.normalized * weight;
    }

    public static Vector3 Blend(Vector3 baseDirection, Vector3 position, SteeringInput input) {
        var cohesionWeight = DefaultCohesionWeight;
        Vector3 cohesionForce = ComputeCohesionForce(position, input.CohesionCenter, cohesionWeight);
        return Vector3.Lerp(baseDirection, cohesionForce, cohesionWeight).normalized;
    }

    public static float ComputeSpeedFactor(Vector3 position, SteeringInput input) {
        float relativePosition = Vector3.Dot(position - input.CohesionCenter, input.AlignmentDirection);
        return relativePosition > 0 ? Mathf.Clamp01(1f - relativePosition * DefaultSpeedAdjustFactor) : 1f;
    }
}

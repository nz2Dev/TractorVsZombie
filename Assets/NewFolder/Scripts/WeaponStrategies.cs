
using System;

using UnityEngine;

[Serializable]
public enum AimType {
    Point,
    Direction
}

[Serializable]
public struct AimConfig {
    public AimType type;
    public float range;
    public float speed;
}

public struct AimInput {
    public Vector3 position;
    public Vector3 targetAim;
    public Vector3 previousAim;
    public float deltaTime;
}

public static class AimStrategy {

    public static Vector3 Evaluate(AimConfig config, AimInput input) {
        return config.type switch {
            AimType.Point => PointAim(config, input),
            AimType.Direction => DirectionAim(config, input),
            _ => input.previousAim,
        };
    }

    private static Vector3 PointAim(AimConfig config, AimInput input) {
        float t = Mathf.Clamp01(input.deltaTime * config.speed);
        return Vector3.Lerp(input.previousAim, input.targetAim, t);
    }

    private static Vector3 DirectionAim(AimConfig config, AimInput input) {
        var toTarget = (input.targetAim - input.position).normalized;
        var currentDir = (input.previousAim - input.position).normalized;
        var newDir = Vector3.Slerp(currentDir, toTarget, input.deltaTime * config.speed);
        return input.position + newDir * config.range;
    }
}
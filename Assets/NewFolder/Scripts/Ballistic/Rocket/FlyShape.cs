using System;

using UnityEngine;

[Serializable]
public struct FlyShape {
    
    public AnimationCurve curve;
    public float amplitude;

    public readonly Vector3 GetPointOnCurve(Vector3 start, Vector3 end, float t) {
        Vector3 horizontal = Vector3.Lerp(start, end, t);
        float height = curve.Evaluate(t) * amplitude;
        return new Vector3(horizontal.x, horizontal.y + height, horizontal.z);
    }

    public readonly Vector3 GetTangent(Vector3 start, Vector3 end, float t) {
        float delta = 0.01f;
        Vector3 p1 = GetPointOnCurve(start, end, t);
        Vector3 p2 = GetPointOnCurve(start, end, Mathf.Min(t + delta, 1f));
        return (p2 - p1).normalized;
    }

}
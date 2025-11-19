using UnityEngine;

public class RocketVisuals : MonoBehaviour {

    [SerializeField] private AnimationCurve flyCurve;
    [SerializeField] private float flyHeight;

    private Vector3 launchPoint;
    private Vector3 landPoint;
    private float startTime;
    private float flyDuration;

    internal void Setup(Vector3 launchPoint, Vector3 landPoint, float startTime, float flyDuration) {
        this.startTime = startTime;
        this.launchPoint = launchPoint;
        this.landPoint = landPoint;
        this.flyDuration = flyDuration;
    }

    private void Update() {
        var progress = (Time.time - startTime) / flyDuration;
        Debug.Log("l: " + launchPoint + " f: " + landPoint + " p: " + progress);
        transform.position = GetPointOnCurve(launchPoint, landPoint, flyCurve, flyHeight, progress);

        var flyTangent = GetTangent(launchPoint, landPoint, flyCurve, flyHeight, progress);
        transform.rotation = Quaternion.LookRotation(flyTangent, Vector3.up);
    }

    Vector3 GetPointOnCurve(Vector3 start, Vector3 end, AnimationCurve curve, float curveScale, float t) {
        Vector3 horizontal = Vector3.Lerp(start, end, t);
        float height = curve.Evaluate(t) * curveScale;
        return new Vector3(horizontal.x, horizontal.y + height, horizontal.z);
    }

    Vector3 GetTangent(Vector3 start, Vector3 end, AnimationCurve curve, float curveScale, float t) {
        float delta = 0.01f;
        Vector3 p1 = GetPointOnCurve(start, end, curve, curveScale, t);
        Vector3 p2 = GetPointOnCurve(start, end, curve, curveScale, Mathf.Min(t + delta, 1f));
        return (p2 - p1).normalized;
    }
}
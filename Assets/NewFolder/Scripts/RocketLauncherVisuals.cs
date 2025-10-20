using System;
using System.Collections.Generic;

using UnityEngine;

public class RocketLauncherVisuals : MonoBehaviour {
    
    private class RocketFly {
        public int rocketId;
        public GameObject visuals;
        public Vector3 launchPoint;
        public Vector3 landPoint;
        public float startTime;
        public float flyDuration;
        public float flyHeight;
    }

    [SerializeField] private Transform launcher;
    [SerializeField] private GameObject rocketVisualsPrefab;
    [SerializeField] private AnimationCurve flyCurve;

    private List<RocketFly> rocketFlies = new ();

    private void Start() {
        rocketFlies = new ();
    }

    public void DestroySelf() {
        for (int i = 0; i < rocketFlies.Count; i++) {
            GameObject.Destroy(rocketFlies[i].visuals);
            rocketFlies.RemoveAt(i);
            i--;
        }
        GameObject.Destroy(gameObject);
    }

    private void Update() {
        for (int i = 0; i < rocketFlies.Count; i++) {
            var rocketFly = rocketFlies[i];
            var visualsTransform = rocketFly.visuals.transform;

            var progress = (Time.time - rocketFly.startTime) / rocketFly.flyDuration;
            visualsTransform.position = GetPointOnCurve(rocketFly.launchPoint, rocketFly.landPoint, flyCurve, rocketFly.flyHeight, progress);

            var flyTangent = GetTangent(rocketFly.launchPoint, rocketFly.landPoint, flyCurve, rocketFly.flyHeight, progress);
            visualsTransform.rotation = Quaternion.LookRotation(flyTangent, Vector3.up);
        }
    }

    public void UpdatePosition(Vector3 position) {
        transform.position = position;
    }

    public void OrientLauncherTowardAim(Vector3 aimPoint, float aimHeight) {
        var flyTangent = GetTangent(launcher.position, aimPoint, flyCurve, aimHeight, 0);
        launcher.rotation = Quaternion.LookRotation(flyTangent, Vector3.up);
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

    internal void ShowRocketFly(int rocketId, RocketTrajectory trajectory, float rocketFlyDuration) {
        var rocketFly = new RocketFly {
            rocketId = rocketId,
            visuals = GameObject.Instantiate(rocketVisualsPrefab, launcher.position, launcher.rotation),
            launchPoint = launcher.position,
            landPoint = trajectory.landPoint,
            startTime = Time.time,
            flyDuration = rocketFlyDuration,
            flyHeight = trajectory.height
        };
        rocketFlies.Add(rocketFly);
    }

    internal void ShowRocketExplosion(int rocketId) {
        for (int i = 0; i < rocketFlies.Count; i++) {
            var rocketFly = rocketFlies[i];
            if (rocketFly.rocketId == rocketId) {
                rocketFlies.RemoveAt(i);
                GameObject.Destroy(rocketFly.visuals);
                return;
            }
        }
    }

}
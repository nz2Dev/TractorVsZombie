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

    private void Update() {
        for (int i = 0; i < rocketFlies.Count; i++) {
            var rocketFly = rocketFlies[i];
            var visualsTransform = rocketFly.visuals.transform;

            var progress = (Time.time - rocketFly.startTime) / rocketFly.flyDuration;
            var height = flyCurve.Evaluate(progress);
            var startToEnd = rocketFly.landPoint - rocketFly.launchPoint;
            visualsTransform.position = rocketFly.launchPoint + startToEnd * progress + Vector3.up * height * rocketFly.flyHeight;

            var slopeVector = GetFlyDirection(startToEnd, rocketFly.flyHeight, flyCurve, progress);
            visualsTransform.rotation = Quaternion.LookRotation(slopeVector, Vector3.up);
        }
    }

    public void OrientLauncherTowardAim(Vector3 aimPoint, float aimHeight) {
        var launcerToAim = aimPoint - transform.position;
        var slopeDirection = GetFlyDirection(launcerToAim, aimHeight, flyCurve, time: 0f);
        launcher.rotation = Quaternion.LookRotation(slopeDirection, Vector3.up);
    }

    private Vector3 GetFlyDirection(Vector3 groundDirection, float maxHeight, AnimationCurve curve, float time, float delta = 0.01f) {
        var lookVector = groundDirection.normalized;
        var slopeVector = GetSlopeVectorNormalized(curve, time, delta) * maxHeight;
        lookVector.y = slopeVector.y;
        return lookVector;
    }

    private static Vector2 GetSlopeVectorNormalized(AnimationCurve curve, float t, float delta = 0.01f) {
        float y1 = curve.Evaluate(t);
        float y2 = curve.Evaluate(t + delta);
        return new Vector2(delta, y2 - y1).normalized;
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
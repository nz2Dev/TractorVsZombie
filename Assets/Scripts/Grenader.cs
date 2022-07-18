using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[SelectionBase]
public class Grenader : MonoBehaviour {
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float fireHeight = 5;
    [SerializeField] private Transform launcherChildGameObject;
    [SerializeField] private float detonateDistance = 0.3f;
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private AnimationCurve flyCurve;

    private void Update() {
        if (Input.GetMouseButton(0)) {
            var camera = Camera.main;
            var clickRay = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(clickRay, out var hitInfo, float.MaxValue, groundLayerMask)) {
                Aim(hitInfo.point);
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            FireLastAimPoint();
        }
    }

    private Vector3 lastAimPoint;
    private Grenade loadedGrenade;

    private void Aim(Vector3 point) {
        Vector3 launcherPosition = launcherChildGameObject.transform.position;
        if (loadedGrenade == null) {
            var greandeObject = Instantiate(grenadePrefab, launcherPosition, Quaternion.identity);
            loadedGrenade = greandeObject.GetComponent<Grenade>();
        }

        lastAimPoint = point;
        transform.LookAt(point, Vector3.up);

        var sampleTimeForAiming = 0.1f;
        var launchToLand = point - launcherPosition;
        var flyPositionLocal = launchToLand * sampleTimeForAiming + flyCurve.Evaluate(sampleTimeForAiming) * fireHeight * Vector3.up;

        var launcherAimPoint = launcherPosition + flyPositionLocal;
        launcherChildGameObject.transform.LookAt(launcherAimPoint, Vector3.up);
    }

    private void FireLastAimPoint() {
        if (lastAimPoint == default) {
            Debug.Log("Has not been aimed");
            return;
        }

        StartCoroutine(FireCoroutine(
            launcherChildGameObject.transform.position,
            lastAimPoint,
            loadedGrenade
        ));

        loadedGrenade = default;
        lastAimPoint = default;
    }

    private IEnumerator FireCoroutine(Vector3 grenadeLaunchPosition, Vector3 grenadeLandPosition, Grenade grenade) {
        var time = 0f;
        var launchToLand = grenadeLandPosition - grenadeLaunchPosition;
        
        while (true) {
            time += Time.deltaTime;
    
            var flyPositionLocal = launchToLand * time + flyCurve.Evaluate(time) * fireHeight * Vector3.up;
            grenade.transform.position = grenadeLaunchPosition + flyPositionLocal;

            if (Vector3.Distance(grenade.transform.position, grenadeLandPosition) < detonateDistance || time >= 1f) {
                grenade.Explode();
                break;
            }
            
            yield return new WaitForEndOfFrame();
        }
    }

    private void OnDrawGizmos() {
        if (lastAimPoint != default) {
            Gizmos.DrawSphere(lastAimPoint, 0.2f);
            Handles.DrawWireDisc(lastAimPoint, Vector3.up, loadedGrenade.EffectRadius);

            const int curveSegments = 10;
            var launchPoint = launcherChildGameObject.transform.position;
            var lastAimPointToLaunchPoint = lastAimPoint - launchPoint;
            
            for (int i = 0; i < curveSegments; i++) {
                var z0 = (float) i / curveSegments;
                var y0 = flyCurve.Evaluate(z0) * fireHeight;

                var z1 = ((float) i + 1) / curveSegments;
                var y1 = flyCurve.Evaluate(z1) * fireHeight;

                Gizmos.DrawLine(
                    launchPoint + lastAimPointToLaunchPoint * z0 + Vector3.up * y0,
                    launchPoint + lastAimPointToLaunchPoint * z1 + Vector3.up * y1
                );
            }
        }
    }
}

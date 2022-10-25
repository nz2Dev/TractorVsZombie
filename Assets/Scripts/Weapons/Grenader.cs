using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[SelectionBase]
public class Grenader : MonoBehaviour {
    [SerializeField] private float fireHeight = 5;
    [SerializeField] private Transform launcherChildGameObject;
    [SerializeField] private float detonateDistance = 0.3f;
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private AnimationCurve flyCurve;

    private Vector3 _aimPoint;
    private SphereExplosion _loadedGrenade;

    public float ExplosionRadius => _loadedGrenade == null ? 0 : _loadedGrenade.EffectRadius;
    public Vector3 LauncherPosition => launcherChildGameObject.transform.position;
    public bool IsInstantiated => _loadedGrenade != null;

    public void InstatiateGrenade() {
        if (_loadedGrenade == null) {
            var launcherPosition = launcherChildGameObject.transform.position;
            var greandeObject = Instantiate(grenadePrefab, launcherPosition, Quaternion.identity);
            _loadedGrenade = greandeObject.GetComponent<SphereExplosion>();
        }
    }

    private void Update() {
        if (_loadedGrenade != null) {
            _loadedGrenade.transform.position = launcherChildGameObject.transform.position;
        }
    }

    public void Aim(Vector3 point) {
        _aimPoint = point;
        transform.LookAt(point, Vector3.up);

        var sampleTimeForAiming = 0.1f;
        var launcherPosition = launcherChildGameObject.transform.position;
        var launchToLand = point - launcherPosition;
        var flyPositionLocal = launchToLand * sampleTimeForAiming + flyCurve.Evaluate(sampleTimeForAiming) * fireHeight * Vector3.up;

        var launcherAimPoint = launcherPosition + flyPositionLocal;
        launcherChildGameObject.transform.LookAt(launcherAimPoint, Vector3.up);
    }

    public void Fire() {
        if (_aimPoint == default || _loadedGrenade == null) {
            Debug.LogWarning("Failed to fire, aimPoint: " + _aimPoint + " loadedGreande: " + _loadedGrenade);
            return;
        }

        StartCoroutine(FireCoroutine(
            launcherChildGameObject.transform.position,
            _aimPoint,
            _loadedGrenade
        ));

        _loadedGrenade = null;
        _aimPoint = default;
    }

    private IEnumerator FireCoroutine(Vector3 grenadeLaunchPosition, Vector3 grenadeLandPosition, SphereExplosion grenade) {
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
        if (_aimPoint != default && _loadedGrenade != null) {
            Gizmos.DrawSphere(_aimPoint, 0.2f);
            Handles.DrawWireDisc(_aimPoint, Vector3.up, _loadedGrenade.EffectRadius);

            const int curveSegments = 10;
            var launchPoint = launcherChildGameObject.transform.position;
            var lastAimPointToLaunchPoint = _aimPoint - launchPoint;
            
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

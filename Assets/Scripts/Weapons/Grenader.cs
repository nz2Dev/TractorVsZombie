using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[SelectionBase]
public class Grenader : MonoBehaviour {

    [SerializeField] private int damage = 50;
    [SerializeField] private float fireHeight = 5;
    [SerializeField] private float explosionRadius = 5;
    [SerializeField] private Transform launcherChildGameObject;
    [SerializeField] private GameObject grenadeProjectilePrefab;
    [SerializeField] private AnimationCurve flyCurve;

    private Vector3 _aimPoint;
    private GrenadeProjectile _loadedGrenade;

    public float ExplosionRadius => explosionRadius;
    public Vector3 LauncherPosition => launcherChildGameObject.transform.position;
    public bool IsInstantiated => _loadedGrenade != null;

    public void InstatiateGrenade() {
        if (_loadedGrenade == null) {
            var launcherPosition = launcherChildGameObject.transform.position;
            var greandeObject = Instantiate(grenadeProjectilePrefab, launcherPosition, Quaternion.identity);
            _loadedGrenade = greandeObject.GetComponent<GrenadeProjectile>();
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
            Debug.LogWarning("Failed to fire, aimPoint: " + _aimPoint + " loadedGrenade: " + _loadedGrenade);
            return;
        }

        var landPosition = _aimPoint;
        _loadedGrenade.Launch(flyCurve, fireHeight, landPosition, (projectile) => {
            var sphereExplosion = projectile.GetComponent<SphereExplosion>();
            sphereExplosion.Explode((epicenter, hits) => {
                foreach (var hit in hits) {
                    if (hit.rigidbody != null) {
                        var health = hit.rigidbody.GetComponent<Health>();
                        var caravanMember = hit.rigidbody.GetComponent<CaravanMember>();
                        if (health != null && caravanMember == null) {
                            var distanceFromEpicentr = Vector3.Distance(hit.rigidbody.transform.position, epicenter);
                            var damageDumping = (int) Utils.Map(distanceFromEpicentr, 0, explosionRadius, 0, damage);
                            health.TakeDamage(damage - damageDumping);
                        }
                    }
                }
            });
        });

        _loadedGrenade = null;
        _aimPoint = default;
    }

    private void OnDrawGizmos() {
        if (_aimPoint != default && _loadedGrenade != null) {
            Gizmos.DrawSphere(_aimPoint, 0.2f);
            Handles.DrawWireDisc(_aimPoint, Vector3.up, explosionRadius);

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

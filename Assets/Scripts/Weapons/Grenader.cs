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
    [SerializeField] private Transform projectilesCollection;
    [SerializeField] private Transform launcherChildGameObject;
    [SerializeField] private String globalAimOutlinesPopulatorName = "GrenadersAimOutlinePopulator";
    [SerializeField] private GameObject grenadeProjectilePrefab;
    [SerializeField] private AnimationCurve flyCurve;

    private NamedPrototypePopulator _aimOutlinesPopulator;
    private GrenadeProjectile _loadedGrenade;
    private Vector3 _lastAimPoint;

    public float ExplosionRadius => explosionRadius;
    public Vector3 LauncherPosition => launcherChildGameObject.transform.position;
    public bool IsAimActivated => _loadedGrenade != null;

    private void Awake() {
        _aimOutlinesPopulator = GameObject.Find(globalAimOutlinesPopulatorName).GetComponent<NamedPrototypePopulator>();
    }

    private void OnDestroy() {
        // TODO better way to use gameObject reference, and clean up automatically 
        // from populator when the link is not valid, e.g link object is destroyed
        _aimOutlinesPopulator.DestroyChild(gameObject.GetInstanceID());
    }

    public void ActivateAim(Vector3 point) {
        if (_loadedGrenade == null) {
            var projectileObject = Instantiate(grenadeProjectilePrefab);
            _loadedGrenade = projectileObject.GetComponent<GrenadeProjectile>();
        } else {
            Debug.LogWarning("Activating aim while loaded grenade already instantiated");
        }

        var launcherPosition = launcherChildGameObject.transform.position;
        _loadedGrenade.transform.position = launcherPosition;
        _loadedGrenade.transform.SetParent(launcherChildGameObject.transform, true);

        var aimOutline = _aimOutlinesPopulator.GetOrCreateChild<AimOutline>(gameObject.GetInstanceID());
        aimOutline.StartOutlining(launcherPosition, point, explosionRadius);

        _lastAimPoint = point;
    }

    public void ChangeAim(Vector3 point) {
        transform.LookAt(point, Vector3.up);

        var sampleTimeForAiming = 0.1f;
        var launcherPosition = launcherChildGameObject.transform.position;
        var launchToLand = point - launcherPosition;
        var flyPositionLocal = launchToLand * sampleTimeForAiming + flyCurve.Evaluate(sampleTimeForAiming) * fireHeight * Vector3.up;

        var launcherAimPoint = launcherPosition + flyPositionLocal;
        launcherChildGameObject.transform.LookAt(launcherAimPoint, Vector3.up);

        var aimOutline = _aimOutlinesPopulator.GetOrCreateChild<AimOutline>(gameObject.GetInstanceID());
        aimOutline.OutlineTarget(launcherPosition, point);

        _lastAimPoint = point;
    }

    public void DeactivateAim() {
        var aimOutline = _aimOutlinesPopulator.GetOrCreateChild<AimOutline>(gameObject.GetInstanceID());
        aimOutline.StopOutlining();

        if (_loadedGrenade != null) {
            Destroy(_loadedGrenade.gameObject);
        }
        _loadedGrenade = null;
        _lastAimPoint = default;
    }

    public void SingleFire() {
        if (_loadedGrenade == null) {
            Debug.LogWarning("Failed to fire, loadedGrenade: " + _loadedGrenade);
            return;
        }

        var shotGrenade = _loadedGrenade;
        _loadedGrenade = null;

        var landPosition = _lastAimPoint;
        _lastAimPoint = default;

        var aimOutline = _aimOutlinesPopulator.GetOrCreateChild<AimOutline>(gameObject.GetInstanceID());
        aimOutline.StopOutlining();

        shotGrenade.transform.SetParent(projectilesCollection, true);
        shotGrenade.Launch(flyCurve, fireHeight, (Vector3)landPosition, (projectile) => {
            var sphereExplosion = projectile.GetComponent<SphereExplosion>();
            sphereExplosion.Explode((Vector3 epicenter, RaycastHit[] hits) => {
                foreach (var hit in hits) {
                    if (hit.rigidbody != null) {
                        var health = hit.rigidbody.GetComponent<Health>();
                        var caravanMember = hit.rigidbody.GetComponent<CaravanMember>();
                        if (health != null && caravanMember == null) {
                            var distanceFromEpicentr = Vector3.Distance(hit.rigidbody.transform.position, epicenter);
                            var damageDumping = (int)Utils.Map(distanceFromEpicentr, 0, explosionRadius, 0, damage);
                            health.TakeDamage(damage - damageDumping);
                        }
                    }
                }
            });
        });
    }
}

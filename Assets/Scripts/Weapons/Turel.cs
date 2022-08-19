using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

[SelectionBase]
public class Turel : MonoBehaviour {
    [SerializeField] private float fireInterval = 2f;
    [SerializeField] [Range(0.1f, 0.98f)] private float repositionAlignmentThreashold = 0.9f;
    [SerializeField] [Range(0.1f, 0.99f)] private float fireAlignmentThreashold = 0.99f;
    [SerializeField] private float turelAlignmentMultiplier = 2f;
    [SerializeField] private int turelDamage = 40;
    [SerializeField] private float firePushMultiplier = 0.5f;
    [SerializeField] private float fireRange = 10f;
    [SerializeField] private Transform fireGunBase;
    [SerializeField] private ProjectileParticles projectileParticles;
    [SerializeField] private CannonParticles cannonParticles;
    [SerializeField] private bool triggerHitDamage = false;
    [SerializeField] private bool collisionHitDamage = false;

    private Animator _animator;
    private float _targetAlignment;
    private Health _targetHealth;

    public float FireRange => fireRange;

    private void Awake() {
        _animator = GetComponent<Animator>();
        projectileParticles.OnAimTriggerHit += OnTriggerHit;
        projectileParticles.OnCollisionHit += OnCollisionHit;
    }

    private void Start() {
        Assert.AreNotApproximatelyEqual(0f, fireInterval);
        projectileParticles.SetFireSpeed(1 / fireInterval);
        cannonParticles.SetEmitSpeed(1 / fireInterval);
    }

    public void StartFire(GameObject targetObject) {
        StopAllCoroutines();

        _targetHealth = targetObject.GetComponent<Health>();
        StartCoroutine(AimingRoutine(targetObject));
        StartCoroutine(FireRoutine(targetObject));
    }

    public void StopFire() {
        StopAllCoroutines();
        StopParticles();
    }

    private IEnumerator FireRoutine(GameObject target) {
        var targetHealth = target.GetComponent<Health>();

        while (targetHealth != null && !targetHealth.IsZero) {
            if (_targetAlignment < repositionAlignmentThreashold) {
                StopParticles();
                yield return WaitForAlignment(target, fireAlignmentThreashold);
                if (target == null) {
                    break;
                }
            }

            StartParticles();
            _animator.SetTrigger("Fire");

            if (!triggerHitDamage && !collisionHitDamage) {
                DealDamage(targetHealth);
            }
            
            yield return new WaitForSeconds(fireInterval);
        }
    }

    private void OnTriggerHit() {
        if (triggerHitDamage && _targetHealth != null) {
            DealDamage(_targetHealth);
        }
    }

    private void OnCollisionHit(GameObject obj) {
        var collidedHealth = obj.GetComponent<Health>();
        if (collisionHitDamage && collidedHealth != null && !collidedHealth.IsZero) {
            DealDamage(collidedHealth);
        }
    }

    private void DealDamage(Health health) {
        var pushForce = Vector3.ProjectOnPlane(transform.forward, Vector3.up) * firePushMultiplier;
        health.transform.position += pushForce;
        health.TakeDamage(turelDamage);
    }

    private void StartParticles() {
        cannonParticles.StartFire();
        projectileParticles.StartFire();
    }

    private void StopParticles() {
        cannonParticles.StopFire();
        projectileParticles.StopFire();
    }

    private IEnumerator WaitForAlignment(GameObject target, float threshold) {
        if (target != null && _targetAlignment < threshold) {
            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator AimingRoutine(GameObject target) {
        var hitTarget = target.GetComponent<HitTarget>();

        while (target != null) {
            var aimPoint = hitTarget != null ? hitTarget.GetAimPoint() : (target.transform.position + Vector3.up);
            projectileParticles.SetAimTriggerPosition(aimPoint);

            var turelToAimPoint = (aimPoint - fireGunBase.position).normalized;
            _targetAlignment = Vector3.Dot(fireGunBase.forward, turelToAimPoint);

            var lookRotation = Quaternion.LookRotation(turelToAimPoint, Vector3.up);
            fireGunBase.rotation = Quaternion.Lerp(fireGunBase.rotation, lookRotation, Time.deltaTime * turelAlignmentMultiplier);

            yield return new WaitForNextFrameUnit(); // works in editor as well
        }
    }
}
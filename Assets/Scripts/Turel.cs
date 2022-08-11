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
    [SerializeField] private ProjectileParticles projectileParticles;
    [SerializeField] private CannonParticles cannonParticles;

    private Animator _animator;
    private float _targetAlignment;

    private void Awake() {
        _animator = GetComponent<Animator>();
    }

    private void Start() {
        Assert.AreNotApproximatelyEqual(0f, fireInterval);
        projectileParticles.SetFireSpeed(1 / fireInterval);
        cannonParticles.SetEmitSpeed(1 / fireInterval);
    }

    public void StartFire(GameObject target) {
        StopAllCoroutines();
        StartCoroutine(AimingRoutine(target));
        StartCoroutine(FireRoutine(target));
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

            var pushForce = Vector3.ProjectOnPlane(transform.forward, Vector3.up) * firePushMultiplier;
            target.transform.position += pushForce;

            targetHealth.TakeDamage(turelDamage);
            yield return new WaitForSeconds(fireInterval);
        }
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
        while (target != null) {
            var turelToTarget = (target.transform.position - transform.position).normalized;
            _targetAlignment = Vector3.Dot(transform.forward, turelToTarget);

            var lookRotation = Quaternion.LookRotation(turelToTarget, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * turelAlignmentMultiplier);

            yield return new WaitForNextFrameUnit(); // works in editor as well
        }
    }
}
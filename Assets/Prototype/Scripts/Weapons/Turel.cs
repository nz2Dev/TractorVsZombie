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
    [SerializeField] private float firePushMultiplier = 0.5f;
    [SerializeField] private float fireRange = 10f;
    [SerializeField] private Transform fireGunBase;
    [SerializeField] private BulletProjectiles projectileParticles;
    [SerializeField] private CannonParticles cannonParticles;
    [SerializeField] private bool triggerHitDamage = false;
    [SerializeField] private bool collisionHitDamage = false;

    private Animator _animator;
    private float _targetAlignment;
    private GameObject _activeTarget;

    public float FireRange => fireRange;

    public event Func<GameObject, bool> OnHit;

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

    public void StartFire(GameObject targetObject, Func<GameObject, bool> haltCondition) {
        StopAllCoroutines();

        _activeTarget = targetObject;
        StartCoroutine(AimingRoutine(targetObject));
        StartCoroutine(FireRoutine(targetObject, haltCondition));
    }

    public void StopFire() {
        StopAllCoroutines();
        StopParticles();
    }

    private IEnumerator FireRoutine(GameObject target, Func<GameObject, bool> haltCondition) {
        while (target != null && !haltCondition(target)) {
            if (_targetAlignment < repositionAlignmentThreashold) {
                StopParticles();
                yield return WaitForAlignment(target, fireAlignmentThreashold);
                if (target == null || haltCondition(target)) {
                    break;
                }
            }

            StartParticles();
            _animator.SetTrigger("Fire");

            if (!triggerHitDamage && !collisionHitDamage) {
                // we don't use projectile callbacks, and deal damage immediately when we alligned
                HitTarget(target);
            }
            
            yield return new WaitForSeconds(fireInterval);
        }
    }

    // we just hit the aimed trigger that we placed on target position in aiming routine, so we have hit the activated target
    private void OnTriggerHit() {
        if (triggerHitDamage && _activeTarget != null) {
            HitTarget(_activeTarget);
        }
    }

    // we hit some collider, but not neccessary our active target, so we notify it anyway, and receiver should decide if it damageble or not
    private void OnCollisionHit(GameObject obj) {
        if (collisionHitDamage) {
            HitTarget(obj);
        }
    }

    private void HitTarget(GameObject target) {
        var pushForce = Vector3.ProjectOnPlane(transform.forward, Vector3.up) * firePushMultiplier;
        if (OnHit?.Invoke(target) == true) {
            target.transform.position += pushForce;
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
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PhysicMember))]
[RequireComponent(typeof(CrowdVehicleDriver))]
public class CylinderZombie : MonoBehaviour, IStabilityListener {

    private Animator _animator;
    private PhysicMember _physicMember;
    private CrowdVehicleDriver _driver;

    private void Awake() {
        _animator = GetComponent<Animator>();
        _physicMember = GetComponent<PhysicMember>();
        _driver = GetComponent<CrowdVehicleDriver>();
    }

    void IStabilityListener.OnStabilityChanged(Rigidbody thisRigidbody, bool stability) {
        thisRigidbody.constraints = stability ? RigidbodyConstraints.FreezeRotation : 0;
        // thisRigidbody.isKinematic = stability;
        _driver.SetPause(!stability);
    }

    public void MovementIdle() {
        if (_chaseCoroutine != null) {
            StopCoroutine(_chaseCoroutine);
        }
        
        _animator.SetTrigger("Idle");
        _driver.SetTarget(null);
        _driver.SetStop(true);
    }

    public void MovementWalk(GameObject target) {
        if (_chaseCoroutine != null) {
            StopCoroutine(_chaseCoroutine);
        }

        _animator.SetTrigger("Idle");
        _driver.SetTarget(target);
        _driver.SetStop(false);
    }

    private Coroutine _chaseCoroutine;

    public void MovementChase(GameObject target, float stopDistance, float resumeDistance, Action onStop, Action onResume) {
        if (_chaseCoroutine != null) {
            StopCoroutine(_chaseCoroutine);
        }

        _chaseCoroutine =
            StartCoroutine(ChaseRoutine(target, stopDistance, resumeDistance, onStop, onResume));
    }

    private IEnumerator ChaseRoutine(GameObject target, float stopDistance, float resumeDistance, Action onStop, Action onResume) {
        _animator.SetTrigger("Idle");
        _driver.SetTarget(target);
        _driver.SetStop(false);

        var chasing = true;
        while (true) {
            var vehiclePosition = _driver.transform.position;
            var targetPosition = _driver.Target.transform.position;
            var distance = Vector3.Distance(targetPosition, vehiclePosition);
            var targetClose = distance < stopDistance;
            var targetFar = distance > resumeDistance;

            if (chasing && targetClose) {
                chasing = false;
                _driver.SetStop(true);
                onStop?.Invoke();
            }

            if (!chasing && targetFar) {
                chasing = true;
                _driver.SetStop(false);
                onResume?.Invoke();
            }

            yield return new WaitForNextFrameUnit();
        }
    }

    private Coroutine _attackCoroutine;

    public void StartAttack(GameObject target, Action<GameObject> onAttack) {
        if (_attackCoroutine != null) {
            StopCoroutine(_attackCoroutine);
        }

        _attackCoroutine = StartCoroutine(AttackRoutine(target, onAttack));
    }

    private IEnumerator AttackRoutine(GameObject target, Action<GameObject> onAttack) {
        while (true) {
            if (target == null) {
                break;
            }

            onAttack?.Invoke(target);
            _animator.SetTrigger("Attack");

            yield return new WaitForSeconds(1);
        }
    }

    public void StopAttack() {
        if (_attackCoroutine != null) {
            StopCoroutine(_attackCoroutine);
            _attackCoroutine = null;
        }
    }

    public void Kill(Action onDeath) {
        _driver.SetTarget(null);
        StopAllCoroutines();
        StartCoroutine(DeathRoutine(onDeath));
    }

    private IEnumerator DeathRoutine(Action onDeathCallback) {
        _animator.ResetTrigger("Idle");
        _animator.SetTrigger("Death");
        yield return new WaitForSeconds(1);
        onDeathCallback?.Invoke();
    }
}

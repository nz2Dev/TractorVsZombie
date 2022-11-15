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
        // state is checked per active coroutine each frame if neccessary
    }

    private Coroutine _movementCoroutine;

    public void MovementIdle() {
        if (!_physicMember.IsStable){
            return;
        }

        if (_movementCoroutine != null) {
            StopCoroutine(_movementCoroutine);
        }
        
        _movementCoroutine = StartCoroutine(IdleRoutine());
    }

    private IEnumerator IdleRoutine() {
        _animator.SetTrigger("Idle");
        _driver.SetTarget(null);
        _driver.SetStop(true);

        while(true) {
            if (!_physicMember.IsStable) {
                // exit point for stability callbacks, when using coroutine as container for state, has no effect, 
                // with polimorfism through interface it will be much clearer
                break;
            }
            yield return new WaitForNextFrameUnit();
        }
    }

    public void MovementWalk(GameObject target) {
        if (!_physicMember.IsStable) {
            return;
        }

        if (_movementCoroutine != null) {
            StopCoroutine(_movementCoroutine);
        }

        _movementCoroutine = StartCoroutine(WalkRoutine(target));
    }

    private IEnumerator WalkRoutine(GameObject target) {
        _animator.SetTrigger("Idle");
        _driver.SetTarget(target);
        _driver.SetStop(false);

        while(true) {
            _driver.SetStop(!_physicMember.IsStable);
            yield return new WaitForNextFrameUnit();
        }
    }

    public void MovementChase(GameObject target, float stopDistance, float resumeDistance, Action onStop, Action onResume, Action onCancelUnstable = null) {
        if (!_physicMember.IsStable) {
            return;
        }

        if (_movementCoroutine != null) {
            StopCoroutine(_movementCoroutine);
        }

        _movementCoroutine =
            StartCoroutine(ChaseRoutine(target, stopDistance, resumeDistance, onStop, onResume, onCancelUnstable));
    }

    private IEnumerator ChaseRoutine(GameObject target, float stopDistance, float resumeDistance, Action onStop, Action onResume, Action onCancelUnstable) {
        _animator.SetTrigger("Idle");
        _driver.SetTarget(target);
        _driver.SetStop(true);
        var chasing = false;

        while (true) {
            if (target == null) {
                _driver.SetTarget(null);
                break;
            }

            if (!_physicMember.IsStable) {
                onCancelUnstable?.Invoke();
                break;
            }

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

    public void StartAttack(GameObject target, Action<GameObject> onAttack, Action onCancelUnstable = null) {
        if (_attackCoroutine != null) {
            StopCoroutine(_attackCoroutine);
        }

        _attackCoroutine = StartCoroutine(AttackRoutine(target, onAttack, onCancelUnstable));
    }

    private IEnumerator AttackRoutine(GameObject target, Action<GameObject> onAttack, Action onCancelUnstable) {
        while (true) {
            if (target == null) {
                _animator.SetTrigger("Cancel");
                break;
            }

            if (!_physicMember.IsStable) {
                _animator.SetTrigger("Cancel");
                onCancelUnstable?.Invoke();
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

        _animator.SetTrigger("Cancel");
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

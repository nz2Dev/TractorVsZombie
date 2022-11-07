using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class CylinderZombie : MonoBehaviour {

    private Coroutine _attackCoroutine;
    private Animator _animator;

    private void Awake() {
        _animator = GetComponentInChildren<Animator>();
    }

    public void StartIdle() {
        _animator.Play("Base Layer.New State");
    }

    public void StartAttack(GameObject target, Action<GameObject> onAttack) {
        if (_attackCoroutine != null) {
            StopCoroutine(_attackCoroutine);
        }

        _attackCoroutine =
            StartCoroutine(AttackRoutine(target, onAttack));
    }

    public void StopAttack() {
        if (_attackCoroutine != null) {
            StopCoroutine(_attackCoroutine);
            _attackCoroutine = null;
        }
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

    public void StartKill(Action onDeath) {
        StartCoroutine(DeathRoutine(onDeath));
    }

    private IEnumerator DeathRoutine(Action onDeathCallback) {
        _animator.SetTrigger("Death");
        yield return new WaitForSeconds(1);
        onDeathCallback?.Invoke();
    }
}

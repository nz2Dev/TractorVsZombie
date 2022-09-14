using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CilinderZombie : MonoBehaviour {

    [SerializeField] private int damage = 55;

    private Coroutine _attackCoroutine;
    private Animator _animator;

    private void Awake() {
        _animator = GetComponentInChildren<Animator>();
    }

    public void StartAttack(GameObject target) {
        if (_attackCoroutine != null) {
            StopCoroutine(_attackCoroutine);
        }

        _attackCoroutine =
            StartCoroutine(AttackRoutine(target));
    }

    public void StopAttack() {
        if (_attackCoroutine != null) {
            StopCoroutine(_attackCoroutine);
            _attackCoroutine = null;
        }
    }

    private IEnumerator AttackRoutine(GameObject target) {
        while (true) {
            if (target == null) {
                break;
            }

            var health = target.GetComponent<Health>();
            if (health == null) {
                // Debug.LogWarning("Can't deal damage to train element without health");
                break;
            }

            health.TakeDamage(damage);
            _animator.SetTrigger("Attack");

            yield return new WaitForSeconds(1);
        }
    }

    public void StartKill() {
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine() {
        _animator.SetTrigger("Death");
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}

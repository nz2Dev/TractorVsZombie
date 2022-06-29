using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Turel : MonoBehaviour {
    
    [SerializeField] private float fireInterval = 2f;
    [SerializeField] private float turelAlignmentMultiplier = 2f;
    [SerializeField] private int turelDamage = 40;
    
    private Animator _animator;

    private Enemy _currentTarget;

    private void Awake() {
        _animator = GetComponent<Animator>();
    }

    private IEnumerator Start() {
        while (true) {
            yield return new WaitForSeconds(0.5f);

            var enemies = FindObjectsOfType<Enemy>();
            var position = transform.position;
            var shortest = enemies.Aggregate((shortest, next) => {
                if (shortest == null) {
                    return next;
                }

                if (Vector3.Distance(next.transform.position, position) <
                    Vector3.Distance(shortest.transform.position, position)) {
                    return next;
                } else {
                    return shortest;
                }
            });

            ChangeTarget(shortest);
        }
    }

    private void ChangeTarget(Enemy enemy) {
        if (_currentTarget != null) {
            return;
        }

        StartCoroutine(nameof(Fire));
        _currentTarget = enemy;
        StartCoroutine(nameof(Fire));
    }

    private IEnumerator Fire() {
        while (true) {
            if (_currentTarget == null) {
                break;
            }

            var targetAlignment = 0f;
            while (targetAlignment < 0.99f) {
                var turelToTarget = (_currentTarget.transform.position - transform.position).normalized;
                Debug.DrawLine(transform.position, _currentTarget.transform.position, Color.blue);

                targetAlignment = Vector3.Dot(transform.forward, turelToTarget);
                var lookRotation = Quaternion.LookRotation(turelToTarget, Vector3.up);
                transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    lookRotation,
                    Time.deltaTime * turelAlignmentMultiplier
                );
                yield return new WaitForNextFrameUnit();
            }

            _animator.SetTrigger("Fire");
            Debug.DrawLine(transform.position, _currentTarget.transform.position, Color.red, 1f);
            var targetHealth = _currentTarget.GetComponent<Health>();
            if (targetHealth != null) {
                targetHealth.TakeDamage(turelDamage);
            }
            
            yield return new WaitForSeconds(fireInterval);
        }
    }
}
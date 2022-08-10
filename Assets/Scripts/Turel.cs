using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[SelectionBase]
public class Turel : MonoBehaviour {
    
    [SerializeField] private float fireInterval = 2f;
    [SerializeField] private float turelAlignmentMultiplier = 2f;
    [SerializeField] private int turelDamage = 40;
    [SerializeField] private float targetSearchInterval = 0.25f;
    [SerializeField] private float firePushMultiplier = 0.5f;
    [SerializeField] private ParticleSystem cannonParticles;
    
    private Animator _animator;

    private Enemy _currentTarget;

    private void Awake() {
        _animator = GetComponent<Animator>();
    }

    private IEnumerator Start() {
        while (true) {
            yield return new WaitForSeconds(targetSearchInterval);

            var enemies = FindObjectsOfType<Enemy>();
            if (enemies.Length <= 0) {
                continue;
            }

            var position = transform.position;
            var shortest = enemies.Aggregate((shortest, next) => {
                if (shortest == null) {
                    return next;
                }

                var shortestHealth = shortest.GetComponent<Health>();
                if (shortestHealth != null && shortestHealth.IsZero) {
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

        StopCoroutine(nameof(Fire));
        _currentTarget = enemy;
        StartCoroutine(nameof(Fire));
    }

    private IEnumerator Fire() {
        while (true) {
            if (_currentTarget == null) {
                cannonParticles.Stop();
                break;
            }

            cannonParticles.Play();
            var targetAlignment = 0f;
            while (targetAlignment < 0.99f) {
                var turelToTarget = (_currentTarget.transform.position - transform.position).normalized;
                //Debug.DrawLine(transform.position, _currentTarget.transform.position, Color.blue);

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
            _currentTarget.transform.position += Vector3.ProjectOnPlane(transform.forward, Vector3.up) * firePushMultiplier;
            
            //Debug.DrawLine(transform.position, _currentTarget.transform.position, Color.red, 1f);
            var targetHealth = _currentTarget.GetComponent<Health>();
            if (targetHealth != null) {
                targetHealth.TakeDamage(turelDamage);
                if (targetHealth.IsZero) {
                    _currentTarget = null;
                    break;
                }
            }
            
            yield return new WaitForSeconds(fireInterval);
        }
    }
}
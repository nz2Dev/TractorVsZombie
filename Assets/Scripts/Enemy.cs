using System;
using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(VehicleDriver))]
public class Enemy : MonoBehaviour {
    [SerializeField] private int damage = 55;
    [SerializeField] private float stopDistance = 0.1f;
    [SerializeField] private float resumeDistance = 0.3f;

    private bool _chasing;
    private VehicleDriver _vehicleDriver;
    private Animator _animator;

    private void Awake() {
        _animator = GetComponentInChildren<Animator>();
        _vehicleDriver = GetComponent<VehicleDriver>();
        
        var health = GetComponent<Health>();
        if (health != null) {
            health.OnHealthChanged += comp => {
                if (comp.IsZero) {
                    StartCoroutine(Death());
                }
            };
        }
    }

    private void Start() {
        StartCoroutine(nameof(SearchTarget));
    }

    private void Update() {
        var vehiclePosition = _vehicleDriver.transform.position;
        var targetPosition = _vehicleDriver.Target.transform.position;
        var distance = Vector3.Distance(targetPosition, vehiclePosition);
        var targetClose = distance < stopDistance;
        var targetFar = distance > resumeDistance;

        if (_chasing && targetClose) {
            _chasing = false;
            _vehicleDriver.Stop();
            StartCoroutine(nameof(Attack));
        }
        
        if (!_chasing && targetFar) {
            _chasing = true;
            _vehicleDriver.Resume();
            StopCoroutine(nameof(Attack));
        }
    }

    private IEnumerator SearchTarget() {
        while (true) {
            yield return new WaitForSeconds(0.5f);

            var elements = FindObjectsOfType<TrainElement>();
            var position = transform.position;
            var shortest = elements.Aggregate((shortest, next) => {
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

            _vehicleDriver.SetTarget(shortest.gameObject);
        }
    }

    private IEnumerator Attack() {
        while (true) {
            if (_vehicleDriver.Target == null) {
                break;
            }

            var trainHealth = _vehicleDriver.Target.GetComponent<Health>();
            if (trainHealth == null) {
                Debug.LogWarning("Can't deal damage to train element without health");
                break;
            }

            trainHealth.TakeDamage(damage);
            _animator.SetTrigger("Attack");

            yield return new WaitForSeconds(1);
        }
    }

    private IEnumerator Death() {
        _animator.SetTrigger("Death");
        
        StopCoroutine(nameof(SearchTarget));
        _vehicleDriver.SetTarget(null);
        
        yield return new WaitForSeconds(1);
        
        Destroy(gameObject);
    }
    
}
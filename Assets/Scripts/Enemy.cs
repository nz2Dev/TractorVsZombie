using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(VehicleDriver))]
public class Enemy : MonoBehaviour {
    [SerializeField] private int damage = 55;

    private VehicleDriver _vehicleDriver;
    private Animator _animator;

    private void Awake() {
        _animator = GetComponentInChildren<Animator>();
        _vehicleDriver = GetComponent<VehicleDriver>();
        _vehicleDriver.OnTargetClose += () => {
            Debug.Log("OnTarget Close");
            StartCoroutine(nameof(Attack));
        };
        _vehicleDriver.OnTargetFar += () => {
            Debug.Log("OnTarget Far");
            StopCoroutine(nameof(Attack));
        };

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
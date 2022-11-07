using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CylinderZombie))]
public class MeleZombieOperator : MonoBehaviour {

    [SerializeField] private int damage = 10;
    [SerializeField] private float stopDistance = 2.0f;
    [SerializeField] private float resumeDistance = 2.1f;
    [SerializeField] private float searchDistance = 100f;
    [SerializeField] private Transform initialPathTarget;

    private CylinderZombie _zombie;
    private CaravanObservable _caravanObservable;

    private Transform _pathTarget;

    public event Action OnDeath;

    private void Awake() {
        _zombie = GetComponent<CylinderZombie>();
        _caravanObservable = FindObjectOfType<CaravanObservable>();

        var health = GetComponent<Health>();
        if (health != null) {
            health.OnHealthChanged += comp => {
                if (comp.IsZero) {
                    StopCoroutine(nameof(SearchTarget));
                    _zombie.Kill(() => {
                        OnDeath?.Invoke();
                    });
                }
            };
        }

        if (initialPathTarget != null) {
            SetPathTarget(initialPathTarget);
        }
    } 

    private void Start() {
        Patrol();
    }

    public void Patrol() {
        StopAllCoroutines();
        StartCoroutine(nameof(SearchTarget));
    }

    public void SetPathTarget(Transform pathTarget) {
        _pathTarget = pathTarget;
    }

    private IEnumerator SearchTarget() {
        while (true) {
            yield return new WaitForSeconds(0.5f);

            var position = transform.position;
            var shortest = (CaravanMember)null;
            var shortestDistance = float.PositiveInfinity;

            if (_caravanObservable != null) {
                foreach (var member in _caravanObservable.CountedMembers) {
                    var distance = Vector3.Distance(position, member.transform.position);
                    if (distance < searchDistance && distance < shortestDistance) {
                        shortestDistance = distance;
                        shortest = member;
                    }
                }
            }

            if (shortest != null) {
                _zombie.MovementChase(
                    shortest.gameObject, 
                    stopDistance, 
                    resumeDistance,
                    onStop: () => {
                        _zombie.StartAttack(shortest.gameObject, (attackedGO) => {
                            var health = attackedGO.GetComponent<Health>();
                            if (health != null) {
                                health.TakeDamage(damage);
                            }
                        });
                    },
                    onResume: () => {
                        _zombie.StopAttack();
                    });
            } else if (_pathTarget != null) {
                _zombie.MovementWalk(
                    _pathTarget.gameObject
                );
            } else {
                _zombie.MovementIdle();
            }
        }
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialEnemyBehaviour : MonoBehaviour {
    [SerializeField] private Transform directionReference;
    [SerializeField] private CaravanObserver caravanObserver;
    [SerializeField] private float checkCaravanInterval = 0.5f;

    private SpecialCrowdDriver _zombieDriver;
    private CylinderZombie _zombie;

    private void Awake() {
        _zombie = GetComponent<CylinderZombie>();
        _zombieDriver = GetComponent<SpecialCrowdDriver>();
    }

    private void Start() {
        _zombieDriver.SetWalkDirection(directionReference.forward);
        StartCoroutine(WaitCaravanRoutine());
    }

    private IEnumerator WaitCaravanRoutine() {
        while (true) {
            yield return new WaitForSeconds(checkCaravanInterval);
            if (caravanObserver.TryGetShortestDistanceMember(transform.position, 1, out var member)) {
                _zombieDriver.SetChaseTransform(member.transform);
                _zombie.StartAttack(member.gameObject);
            } else {
                _zombieDriver.SetChaseTransform(null);
                _zombie.StopAttack();
            }
        }
    }
}

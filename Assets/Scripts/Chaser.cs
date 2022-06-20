using System;
using UnityEngine;

public class Chaser : MonoBehaviour {
    [SerializeField] private GameObject target;

    [SerializeField] private float stopDistance = 1;
    [SerializeField] private float resumeDistance = 1.5f;

    private bool _chasing;

    public GameObject Target => target;

    public event Action OnTargetClose;
    public event Action OnTargetFar;

    public void SetTarget(GameObject newTarget) {
        target = newTarget;
    }

    private void Update() {
        if (target == null) {
            return;
        }

        var transformPosition = transform.position;
        var targetPosition = target.transform.position;
        var distance = Vector3.Distance(targetPosition, transformPosition);
        var targetClose = distance < stopDistance;
        var targetFar = distance > resumeDistance;

        if (_chasing && targetClose) {
            _chasing = false;
            OnTargetClose?.Invoke();
        }

        if (!_chasing && targetFar) {
            _chasing = true;
            OnTargetFar?.Invoke();
        }

        if (!_chasing) {
            return;
        }
        
        var direction = targetPosition - transformPosition;
        direction.Normalize();

        transformPosition += direction * Time.deltaTime;
        transform.position = transformPosition;

        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }
}
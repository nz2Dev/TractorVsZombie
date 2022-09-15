using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialCrowdDriver : MonoBehaviour {

    [SerializeField] private float separationCheckRadius = 3f;
    [SerializeField] private LayerMask neighborsLayerMask;

    private Vector3 _walkDirection;
    private Transform _chaseTransform;
    private Vehicle _vehicle;

    private void Awake() {
        _vehicle = GetComponent<Vehicle>();
    }

    public void SetWalkDirection(Vector3 direction) {
        _walkDirection = direction.normalized;
    }

    public void SetChaseTransform(Transform chaseTransform) {
        _chaseTransform = chaseTransform;
    }

    private void Update() {
        var arrivalForce = Vector3.zero;
        if (_chaseTransform != null) {
            arrivalForce = _vehicle.Arrival(_chaseTransform.position, 1f);
        } else if (_walkDirection.sqrMagnitude > 0) {
            arrivalForce = _vehicle.Arrival(transform.position + _walkDirection * 2f, 1f);
        }
        _vehicle.ApplyForce(arrivalForce, "Arrival", Color.blue);

        if (_vehicle.TrySeparateInsideSphere(separationCheckRadius, neighborsLayerMask, out var separationForce)) {
            _vehicle.ApplyForce(separationForce, "Separation", Color.magenta);
        }
    }
}

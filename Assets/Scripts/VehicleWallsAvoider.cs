using System.Collections;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Vehicle))]
public class VehicleWallsAvoider : MonoBehaviour {

    [SerializeField] private int checkDistance = 3;
    [SerializeField] private LayerMask wallsLayerMask;
    [SerializeField] private float forceWeight = 1f;
    private Vehicle _vehicle;

    private void Awake() {
        _vehicle = GetComponent<Vehicle>();
    }

    private Vector3 desiredVelocityDebug;

    private void Update() {
        if (Physics.Raycast(transform.position, transform.forward, out var hitInfo, checkDistance, wallsLayerMask)) {
            var dotForwardToNormal = Vector3.Dot(hitInfo.normal, transform.forward);
            if (dotForwardToNormal < 0) {
                var desiredVelocity = transform.forward + hitInfo.normal * 2 * Mathf.Abs(dotForwardToNormal);
                desiredVelocityDebug = desiredVelocity;

                var steeringForce = desiredVelocity - _vehicle.Velocity;
                var steeringForceNormalized = Vector3.ClampMagnitude(steeringForce, _vehicle.MaxForce) * forceWeight;
                _vehicle.ApplyForce(steeringForceNormalized, "WallAvoid", Color.yellow);
            }
        }
    }

    private void OnDrawGizmosSelected() {
        Handles.DrawWireDisc(transform.position, Vector3.up, checkDistance);
    }

}
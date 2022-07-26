using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Vehicle))]
public class VehicleWallsAvoidSteering : MonoBehaviour {

    [SerializeField] private float checkDistance = 3;
    [SerializeField] private float sphereRadius = 0.5f;
    [SerializeField] private LayerMask wallsLayerMask;
    [SerializeField] private float forceWeight = 1f;
    private Vehicle _vehicle;

    private void Awake() {
        _vehicle = GetComponent<Vehicle>();
    }

    private Vector3 desiredVelocityDebug;

    private void Update() {
        // var hits = Physics.SphereCastAll(transform.position, checkDistance, Vector3.up, 10f, wallsLayerMask);
        // Debug.Log("spherecastAll hist: " + hits.Length);
        // if (hits.Length > 0) {
        //     RaycastHit firstHit = hits[0];
        //     Debug.DrawRay(firstHit.point, firstHit.normal, Color.blue);

        //     var closestPoint = firstHit.collider.ClosestPoint(transform.position);
        //     Debug.DrawRay(closestPoint, firstHit.normal, Color.red);

        //     var boxCollider = (BoxCollider) firstHit.collider;
        //     boxCollider.
        // }

        var walls = Physics.OverlapSphere(transform.position, checkDistance, wallsLayerMask);
        foreach (var wall in walls) {
            var awayVector = (transform.position - wall.ClosestPoint(transform.position));
            awayVector = Vector3.ProjectOnPlane(awayVector, Vector3.up).normalized;
            Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
            var dotForwardToNormal = Vector3.Dot(awayVector, flatForward);
            var desiredVelocity = flatForward + awayVector * 2 * Mathf.Abs(dotForwardToNormal);
            var steerAway = desiredVelocity - _vehicle.Velocity;
            _vehicle.ApplyForce(steerAway, "SteerAway-" + wall.name, Color.yellow);
        }

        // if (Physics.Raycast(transform.position, transform.forward, out var hitInfo, checkDistance, wallsLayerMask)) {
        //     Debug.DrawRay(hitInfo.point, hitInfo.normal);

        //     var desiredVelocity = -Vector3.ProjectOnPlane(-transform.forward, hitInfo.normal);
        //     desiredVelocity.y = 0;
        //     desiredVelocity.Normalize();
        //     Debug.DrawRay(hitInfo.point, desiredVelocity, Color.red);

            // var dotForwardToNormal = Vector3.Dot(hitInfo.normal, transform.forward);
            // var desiredVelocity = transform.forward + hitInfo.normal * 2 * Mathf.Abs(dotForwardToNormal);

        //     var steeringForce = desiredVelocity - _vehicle.Velocity;
        //     var steeringForceNormalized = Vector3.ClampMagnitude(steeringForce, _vehicle.MaxForce) * forceWeight;
        //     _vehicle.ApplyForce(steeringForceNormalized, "WallAvoid", Color.yellow);
        // }
    }

    private void OnDrawGizmosSelected() {
        Handles.DrawWireDisc(transform.position, Vector3.up, checkDistance);
    }

}
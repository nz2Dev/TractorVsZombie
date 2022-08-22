using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class VehicleWallsSlideSteering : MonoBehaviour {
    
    [SerializeField] private float checkDistance = 1f;
    [SerializeField] private LayerMask wallsLayerMask;
    [SerializeField] private float forceWeight = 2f;

    private Vehicle _vehicle;

    private void Awake() {
        _vehicle = GetComponent<Vehicle>();
    }

    private void Update() {
        var position = transform.position;
        var walls = Physics.OverlapSphere(position, checkDistance, wallsLayerMask);
        foreach (var wall in walls) {
            Vector3 closestPointOnWall = wall.ClosestPoint(position);
            var wallToVehicleVector = position - closestPointOnWall;
            // Debug.DrawRay(wall.ClosestPoint(position), wallToVehicleVector, Color.blue, 0.1f);
            var wallToVehicleNormal = wallToVehicleVector.normalized;
            var forwardOnWallProjection = Vector3.ProjectOnPlane(transform.forward, wallToVehicleNormal);
            // Debug.Log("projection: " + forwardOnWallProjection + " magnitued: " + forwardOnWallProjection.magnitude + " normalized: " + forwardOnWallProjection.normalized);
            
            var slideDirection = (Vector3) default;
            if (forwardOnWallProjection.magnitude > Vector3.kEpsilon) {
                slideDirection = forwardOnWallProjection.normalized;
            } else {
                slideDirection = transform.right;
            }
            // Debug.DrawRay(position, slideDirection, Color.white, 0.1f);

            var seekPosition = closestPointOnWall + wallToVehicleNormal * checkDistance + slideDirection;
            Debug.DrawLine(position, seekPosition);

            var steeringForce = _vehicle.CalculateSeekSteeringForce(seekPosition) * forceWeight;
            _vehicle.ApplyForce(steeringForce, "WallSlide", Color.magenta);
        }
    }

    private void OnDrawGizmosSelected() {
        Handles.DrawWireDisc(transform.position, Vector3.up, checkDistance);
    }

}
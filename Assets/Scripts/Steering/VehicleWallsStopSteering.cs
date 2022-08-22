using UnityEditor;
using UnityEngine;

public class VehicleWallsStopSteering : MonoBehaviour {
    [SerializeField] private float checkDistance = 1.5f;
    [SerializeField] private LayerMask wallsLayerMask;

    private Vehicle _vehicle;

    private void Awake() {
        _vehicle = GetComponent<Vehicle>();
    }

    private void Update() {
        if (Physics.Raycast(transform.position, transform.forward, out var hitInfo, checkDistance, wallsLayerMask)) {
            var stoppingForce = _vehicle.Arrival(hitInfo.point, checkDistance);
            _vehicle.ApplyForce(stoppingForce, "WallStop", Color.gray);
        }
    }

    private void OnDrawGizmosSelected() {
        Handles.color = Color.gray;
        Handles.DrawWireDisc(transform.position, Vector3.up, checkDistance);
    }
}
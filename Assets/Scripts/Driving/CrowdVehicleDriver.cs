using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CrowdVehicleDriver : MonoBehaviour {
    [SerializeField] private float arrivalWeight = 1f;
    [SerializeField] private float arrivalSlowingDistance = 1.5f;
    [SerializeField] private GameObject arrivalTarget;

    [SerializeField] private float wallsCheckRadius = 3;
    [SerializeField] private LayerMask wallsLayerMask;
    [SerializeField] private float wallsCheckWeight = 1;

    [SerializeField] private float separationCheckRadius = 3f;
    [SerializeField] private LayerMask neighborsLayerMask;
    [SerializeField] private float separationWeigh = 1f;

    private Vehicle _vehicle;
    private Collider[] _separationAllocation = new Collider[50];
    private Collider[] _wallsAvoidAllocation = new Collider[10];

    public GameObject Target => arrivalTarget;

    private void Awake() {
        _vehicle = GetComponent<Vehicle>();
    }

    public void Stop() {
        _vehicle.enabled = false;
    }

    public void Resume() {
        _vehicle.enabled = true;
    }

    public void SetTarget(GameObject newTarget) {
        _vehicle.enabled = newTarget != null;
        arrivalTarget = newTarget;
    }

    private void Update() {
        if (arrivalTarget == null) {
            return;
        }

        if (_vehicle.TrySeparateInsideSphere(separationCheckRadius, neighborsLayerMask, _separationAllocation, out var separationForce)) {
            _vehicle.ApplyForce(separationForce, "Separation", Color.magenta);
        }

        if (_vehicle.TryAvoidWallsAround(wallsCheckRadius, wallsLayerMask, _wallsAvoidAllocation, out var wallAvoidanceForce)) {
            _vehicle.ApplyForce(wallAvoidanceForce, "AvoidWalls", Color.yellow);
        }

        var arrivalForce = _vehicle.Arrival(arrivalTarget.transform.position, arrivalSlowingDistance);
        _vehicle.ApplyForce(arrivalForce * arrivalWeight, "Arrival", Color.blue);
    }

    private void OnDrawGizmosSelected() {
        //Handles.DrawWireDisc(transform.position, Vector3.up, wallsCheckDistance);
    }
}
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

    private bool _pause;
    private bool _stop;

    public GameObject Target => arrivalTarget;

    private void Awake() {
        _vehicle = GetComponent<Vehicle>();
    }

    public void SetPause(bool pause) {
        _pause = pause;
        UpdateVehicleState();
    }

    public void SetStop(bool stop) {
        _stop = stop;
        UpdateVehicleState();
    }

    public void SetTarget(GameObject newTarget) {
        arrivalTarget = newTarget;
        UpdateVehicleState();
    }

    private void UpdateVehicleState() {
        _vehicle.enabled = !_pause && !_stop && arrivalTarget != null;
    }

    private void Update() {
        if (arrivalTarget == null || !_vehicle.enabled) {
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
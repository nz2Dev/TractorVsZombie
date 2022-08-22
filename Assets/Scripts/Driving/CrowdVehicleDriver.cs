using System;
using System.Linq;
using UnityEngine;

public class CrowdVehicleDriver : MonoBehaviour {

    [SerializeField] private float arrivalWeight = 1f;
    [SerializeField] private float arrivalSlowingDistance = 1.5f;
    [SerializeField] private GameObject arrivalTarget;

    private Vehicle _vehicle;

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

        var arrivalForce = _vehicle.Arrival(arrivalTarget.transform.position, arrivalSlowingDistance);
        _vehicle.ApplyForce(arrivalForce * arrivalWeight, "Arrival", Color.blue); // * 1.5f
    }
}
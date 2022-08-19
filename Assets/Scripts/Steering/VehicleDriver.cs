using System;
using System.Linq;
using UnityEngine;

public class VehicleDriver : MonoBehaviour {
        
    private Vehicle _vehicle;
    private VehicleTargetSteering targetSteering;

    public GameObject Target => targetSteering.Target;

    private void Awake() {
        _vehicle = GetComponent<Vehicle>();
        targetSteering = GetComponent<VehicleTargetSteering>();
    }

    public void Stop() {
        _vehicle.enabled = false;
    }

    public void Resume() {
        _vehicle.enabled = true;
    }

    public void SetTarget(GameObject newTarget) {
        _vehicle.enabled = newTarget != null;
        targetSteering.SetTarget(newTarget);
    }
}
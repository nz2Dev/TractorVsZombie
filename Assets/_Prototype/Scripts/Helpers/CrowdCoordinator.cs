using System;
using UnityEngine;

public class CrowdCoordinator : MonoBehaviour {
    
    [SerializeField] private LayerMask groundMask;
    
    public GameObject cursor;
    public GameObject obstacle;

    private void Start() {
        foreach (var vehicle in FindObjectsOfType<CrowdVehicleDriver>()) {
            vehicle.SetTarget(cursor);
        }
    }

    private void Update() {
        if (Input.GetMouseButton(0)) {
            var screenPointToRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(screenPointToRay, out var hit, 1000, groundMask)) {
                cursor.transform.position = hit.point;
            }
        }

        if (Input.GetMouseButton(1)) {
            var screenPointToRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(screenPointToRay, out var hit, 1000, groundMask)) {
                obstacle.transform.position = hit.point;
            } 
        }
    }
}
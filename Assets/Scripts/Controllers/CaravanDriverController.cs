using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CaravanDriverController : MonoBehaviour {

    [SerializeField] private InputActionReference steeringEngage;
    [SerializeField] private InputActionReference handbreakToggle;
    [SerializeField] private InputActionReference steeringAxis;
    [SerializeField] private InputActionReference moveAxis;
    [SerializeField] private WheeledVehicleDriver driver;
    [SerializeField] private Transform drivingPOV;
    [SerializeField] private bool localSpace = false;

    private bool mouseDriving;

    private void Update() {
        if (driver == null) {
            return;
        }

        if (handbreakToggle.action.WasPerformedThisFrame()) {
            driver.SetHandbreak(!driver.IsHandbreak);
        }

        if (steeringEngage.action.inProgress && steeringEngage.action.WasPressedThisFrame()) {
            mouseDriving = true;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (mouseDriving) {
            var xAxis = steeringAxis.action.ReadValue<Vector2>().x;
            var angleRad = Mathf.Clamp(xAxis * Mathf.Deg2Rad, -Mathf.PI / 2, Mathf.PI / 2);
            var directionDriverSpace = new Vector3(Mathf.Sin(angleRad), 0, Mathf.Cos(angleRad));
            var steerMouseDir = driver.transform.TransformDirection(directionDriverSpace);
            driver.SetSteerDirection(steerMouseDir);
        }

        if (steeringEngage.action.WasPerformedThisFrame() && steeringEngage.action.WasReleasedThisFrame()) {
            mouseDriving = false;
            Cursor.lockState = CursorLockMode.None;
            return;
        }

        var inputAxis = moveAxis.action.ReadValue<Vector2>();
        var inputVector = new Vector3(inputAxis.x, 0, inputAxis.y);
        if (inputVector.sqrMagnitude < float.Epsilon) {
            return;
        }

        var steerDirection = inputVector;
        if (localSpace) {
            var perspectiveDirection = drivingPOV.TransformDirection(new Vector3(inputAxis.x, inputAxis.y, 0));
            var perspectiveTurnDirection = Vector3.ProjectOnPlane(perspectiveDirection, driver.transform.up).normalized;
            steerDirection = perspectiveTurnDirection;
        }

        driver.SetSteerDirection(steerDirection);
    }

}
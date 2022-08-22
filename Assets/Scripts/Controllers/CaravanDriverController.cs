using System;
using UnityEngine;

public class CaravanDriverController : MonoBehaviour {

    [SerializeField] private AnchoredDirectionVehicleDriver driver;
    [SerializeField] private Transform drivingPOV;
    [SerializeField] private bool localSpace = false;

    private bool mouseDriving;

    private void Update() {
        if (driver == null) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q)) {
            driver.SetHandbreak(!driver.IsHandbreak);
        }

        if (Input.GetMouseButtonDown(1)) {
            mouseDriving = true;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (mouseDriving) {
            var xAxis = Input.GetAxisRaw("Mouse X");
            var angleRad = Mathf.Clamp(xAxis, -Mathf.PI / 2, Mathf.PI / 2);
            var directionDriverSpace = new Vector3(Mathf.Sin(angleRad), 0, Mathf.Cos(angleRad));
            var steerMouseDir = driver.transform.TransformDirection(directionDriverSpace);
            driver.SetSteerDirection(steerMouseDir);
        }

        if (Input.GetMouseButtonUp(1)) {
            mouseDriving = false;
            Cursor.lockState = CursorLockMode.None;
            return;
        }

        var horizontalAxis = Input.GetAxisRaw("Horizontal");
        var verticalAxis = Input.GetAxisRaw("Vertical");
        var inputVector = new Vector3(horizontalAxis, 0, verticalAxis);
        if (inputVector.sqrMagnitude < float.Epsilon) {
            return;
        }

        var steerDirection = inputVector;
        if (localSpace) {
            var perspectiveDirection = drivingPOV.TransformDirection(inputVector);
            var perspectiveTurnDirection = Vector3.ProjectOnPlane(perspectiveDirection, driver.transform.up).normalized;
            steerDirection = perspectiveTurnDirection;
        }

        driver.SetSteerDirection(steerDirection);
    }

}
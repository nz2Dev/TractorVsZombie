using UnityEngine;

public class CaravanDriverController : MonoBehaviour {

    [SerializeField] private AnchoredDirectionVehicleDriver driver;
    [SerializeField] private Transform drivingPOV;
    [SerializeField] private bool localSpace = false;

    private void Update() {
        if (driver == null) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q)) {
            driver.SetHandbreak(!driver.IsHandbreak);
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
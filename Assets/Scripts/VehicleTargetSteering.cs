using UnityEngine;

public class VehicleTargetSteering : MonoBehaviour {

    [SerializeField] private GameObject target;
    [SerializeField] private float slowingDistance = 1.5f;
    [SerializeField] private float steeringWeight = 1f;

    private Vehicle _vehicle;

    public GameObject Target => target;

    private void Awake() {
        _vehicle = GetComponent<Vehicle>();
    }

    public void SetTarget(GameObject target) {
        this.target = target;
    }

    private void Update() {
        if (target == null) {
            return;
        }

        var arrivalForce = CalculateArrivalSteeringForce(target.transform.position);
        _vehicle.ApplyForce(arrivalForce * steeringWeight, "Arrival", Color.blue); // * 1.5f
    }

    private Vector3 CalculateArrivalSteeringForce(Vector3 targetPosition) {
        var vehiclePosition = _vehicle.transform.position;
        var distance = Vector3.Distance(targetPosition, vehiclePosition);
        var rampedSpeed = _vehicle.MaxSpeed * (distance / slowingDistance);
        var clippedSpeed = Mathf.Min(rampedSpeed, _vehicle.MaxSpeed);
        var desiredVelocity = (clippedSpeed / distance) * (targetPosition - vehiclePosition);
        return desiredVelocity - _vehicle.Velocity;
    }

}
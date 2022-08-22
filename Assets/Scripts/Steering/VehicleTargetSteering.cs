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

        var arrivalForce = _vehicle.CalculateArrivalSteeringForce(target.transform.position, slowingDistance);
        _vehicle.ApplyForce(arrivalForce * steeringWeight, "Arrival", Color.blue); // * 1.5f
    }

}
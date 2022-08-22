using System.Linq;
using UnityEngine;

public class VehicleAnchoredDirectionSteering : MonoBehaviour {

    [SerializeField] private float lookaheadDistance = 1;
    [SerializeField] private bool handbreak = false;

    private Vehicle _vehicle;
    private Vector3 _turnDirection;
    private Vector3 _turnAnchor;

    public bool IsHandbreak => handbreak;
    public Vector3 TurnDirection => _turnDirection;

    private void Awake() {
        _vehicle = GetComponentInChildren<Vehicle>();
    }

    private void Update() {
        if (handbreak) {
            HandbreakVehicle();
            return;
        }

        if (_turnDirection != default) {
            SteerToDirection();
        }
    }

    public void SetHandbreak(bool handbreak) {
        this.handbreak = handbreak;
    }

    public void SetDirection(Vector3 direction, Vector3 anchor) {
        _turnDirection = direction;
        _turnAnchor = anchor;
    }

    private void HandbreakVehicle() {
        _vehicle.ApplyForce(-_vehicle.Velocity, "Break", Color.black);
    }

    private void SteerToDirection() {
        var anchorToPosition = transform.position - _turnAnchor;
        var vehiclePositionOnTurnDirection = Vector3.Project(anchorToPosition, _turnDirection);
        var futurePosition = _turnAnchor + vehiclePositionOnTurnDirection + _turnDirection * lookaheadDistance;
        var baseSteeringForce = _vehicle.Seek(futurePosition);
        _vehicle.ApplyForce(baseSteeringForce, "AnchoredDirectional", Color.gray);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_turnAnchor, 0.1f);
        Gizmos.DrawRay(_turnAnchor, _turnDirection);
    }
}
using System.Linq;
using UnityEngine;

public class AnchoredDirectionVehicleDriver : MonoBehaviour {

    [SerializeField] private bool snapAnchorPosition = true;
    [SerializeField] private bool reanchorEachInput = false;
    [SerializeField][Range(0, 1f)] private float turnBaseOffset;
    [SerializeField] private Transform anchorCheckPoint;
    [SerializeField] private float lookaheadDistance = 1;
    [SerializeField] private bool handbreak = false;

    private Vehicle _vehicle;
    private Vector3 _turnDirection;
    private Vector3 _turnAnchor;

    public bool IsHandbreak => handbreak;

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

    public void SetSteerDirection(Vector3 direction) {
        if (direction.sqrMagnitude > 0) {
            if (reanchorEachInput || direction != _turnDirection) {
                _turnAnchor = GetBasePosition();
                _turnDirection = GetBaseDirection(direction);
            }
        }
    }

    private Vector3 GetBaseDirection(Vector3 direction) {
        if (!snapAnchorPosition) {
            return direction;
        }

        var forwardDot = Vector3.Dot(Vector3.forward, direction);
        var rightDot = Vector3.Dot(Vector3.right, direction);
        return Mathf.Abs(forwardDot) > Mathf.Abs(rightDot) ? Vector3.forward * Mathf.Sign(forwardDot) : Vector3.right * Mathf.Sign(rightDot);
    }

    private void HandbreakVehicle() {
        _vehicle.ApplyForce(-_vehicle.Velocity, "Break", Color.black);
    }

    private void SteerRelativeToCamera(Vector3 inputVector) {
        var turnDirectionWorld = Camera.main.transform.TransformDirection(inputVector);
        turnDirectionWorld = Vector3.ProjectOnPlane(turnDirectionWorld, Vector3.up);

        var steeringFoce = _vehicle.CalculateSeekSteeringForce(_vehicle.transform.position + turnDirectionWorld);
        _vehicle.ApplyForce(steeringFoce, "Input", Color.gray);
    }

    private void SteerToDirection() {
        var anchorToPosition = transform.position - _turnAnchor;
        var vehiclePositionOnTurnDirection = Vector3.Project(anchorToPosition, _turnDirection);
        var futurePosition = _turnAnchor + vehiclePositionOnTurnDirection + _turnDirection * lookaheadDistance;
        var baseSteeringForce = _vehicle.CalculateSeekSteeringForce(futurePosition);
        _vehicle.ApplyForce(baseSteeringForce, "AnchoredDirectional", Color.gray);
    }

    private Vector3 ClosestPointOnVector(Vector3 vector, Vector3 sourcePoint) {
        var ap = sourcePoint;
        var ab = vector;
        ab.Normalize();
        var normalPointOnSegment = ab * Vector3.Dot(ap, ab);
        return normalPointOnSegment;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(_turnAnchor, 0.1f);
        Gizmos.DrawRay(_turnAnchor, _turnDirection);
    }

    private Vector3 GetBasePosition() {
        var position = transform.position;
        if (!snapAnchorPosition) {
            return position;
        }

        return SimpleBasePosition(anchorCheckPoint.position);
    }

    private Vector3 SimpleBasePosition(Vector3 position) {
        position.x = Mathf.Floor(position.x) + turnBaseOffset;
        position.z = Mathf.Floor(position.z) + turnBaseOffset;
        return position;
    }

    private float baseAxis(float axis, float direction) {
        if (direction > 0) {
            var flooredAxis = Mathf.Floor(axis);
            var difference = axis - flooredAxis;
            if (difference > 0.5f) {
                return flooredAxis + 1 + turnBaseOffset;
            } else {
                return flooredAxis + turnBaseOffset;
            }
        } else {
            var ceiledAxis = Mathf.Ceil(axis);
            var difference = ceiledAxis - axis;
            if (difference > 0.5f) {
                return ceiledAxis - 1 - turnBaseOffset;
            } else {
                return ceiledAxis - turnBaseOffset;
            }
        }
    }
}
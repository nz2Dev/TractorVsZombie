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

    private void Awake() {
        _vehicle = GetComponentInChildren<Vehicle>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            handbreak = !handbreak;
        }

        if (handbreak) {
            HandbreakVehicle();
            return;
        }

        var horizontalAxis = Input.GetAxisRaw("Horizontal");
        var verticalAxis = Input.GetAxisRaw("Vertical");

        var inputVector = new Vector3(horizontalAxis, 0, verticalAxis);
        if (inputVector.sqrMagnitude > 0) {
            if (reanchorEachInput || inputVector != _turnDirection) {
                _turnAnchor = GetBasePosition();
                _turnDirection = inputVector;
            }
        }

        if (_turnDirection != default) {
            SteerToDirection();
        }
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
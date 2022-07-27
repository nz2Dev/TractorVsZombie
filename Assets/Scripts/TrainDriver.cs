using UnityEngine;

public class TrainDriver : MonoBehaviour {

    [SerializeField][Range(0, 1f)] private float turnBaseOffset;
    [SerializeField] private float lookaheadDistance = 1;

    private Vehicle _vehicle;
    private Vector3 _turnDirection;

    private void Awake() {
        _vehicle = GetComponent<Vehicle>();
    }

    private void Update() {
        if (Input.GetKey(KeyCode.Q)) {
            BreakVehicle();
            return;
        }

        var horizontalAxis = Input.GetAxisRaw("Horizontal");
        var verticalAxis = Input.GetAxisRaw("Vertical");

        var inputVector = new Vector3(horizontalAxis, 0, verticalAxis);
        if (inputVector.sqrMagnitude > 0) {
            _turnDirection = inputVector;
        }

        if (_turnDirection != default) {
            SteerAtSnapDirection(_turnDirection);
        }
    }

    private void BreakVehicle() {
        _vehicle.ApplyForce(-_vehicle.Velocity, "Break", Color.black);
    }

    private void SteerRelativeToCamera(Vector3 inputVector) {
        var turnDirectionWorld = Camera.main.transform.TransformDirection(inputVector);
        turnDirectionWorld = Vector3.ProjectOnPlane(turnDirectionWorld, Vector3.up);

        var steeringFoce = _vehicle.CalculateSeekSteeringForce(_vehicle.transform.position + turnDirectionWorld);
        _vehicle.ApplyForce(steeringFoce, "Input", Color.gray);
    }

    private void SteerAtSnapDirection(Vector3 inputVector) {
        var basePosition = GetBasePosition();
        var facingVector = ((basePosition + inputVector) - transform.position).normalized;
        var futurePosition = transform.position + facingVector * lookaheadDistance;
        var projectPosition = futurePosition - basePosition;
        var seekVector = Vector3.Project(projectPosition, inputVector);// ClosestPointOnVector(inputVector, projectPosition);

        var baseSteeringForce = _vehicle.CalculateSeekSteeringForce(basePosition + seekVector);
        _vehicle.ApplyForce(baseSteeringForce, "InputSnapped", Color.gray);
    }

    private Vector3 ClosestPointOnVector(Vector3 vector, Vector3 sourcePoint) {
        var ap = sourcePoint;
        var ab = vector;
        ab.Normalize();
        var normalPointOnSegment = ab * Vector3.Dot(ap, ab);
        return normalPointOnSegment;
    }

    private void OnDrawGizmosSelected() {
        Vector3 basePosition = GetBasePosition();
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(basePosition, 0.1f);
    }

    private Vector3 GetBasePosition() {
        var position = transform.position;
        position.x = Mathf.Floor(position.x);
        position.z = Mathf.Floor(position.z);
        position += new Vector3(turnBaseOffset, 0, turnBaseOffset);
        return position;
    }
}
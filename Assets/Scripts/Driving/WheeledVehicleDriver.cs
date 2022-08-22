using Unity.VisualScripting;
using UnityEngine;

public class WheeledVehicleDriver : MonoBehaviour {
    
    [SerializeField] private bool snapAnchorPosition = true;
    [SerializeField] private bool reanchorEachInput = false;
    [SerializeField][Range(0, 1f)] private float turnBaseOffset = 0.5f;
    [SerializeField] private Transform anchorCheckPoint;
    [SerializeField] private float lookaheadDistance = 1f;
    [SerializeField] private LayerMask wallsLayerMask;
    [SerializeField] private bool handbreak;

    private Vehicle _vehicle;
    private Vector3 _turnDirection;
    private Vector3 _turnAnchor;

    public bool IsHandbreak => handbreak;

    private void Awake() {
        _vehicle = GetComponent<Vehicle>();
    }

    public void SetHandbreak(bool handbreak) {
        this.handbreak = handbreak;
    }

    public void SetSteerDirection(Vector3 direction) {
        if (direction.sqrMagnitude > 0) {
            if (reanchorEachInput || direction != _turnDirection) {
                _turnDirection = GetBaseDirection(direction);
                _turnAnchor = GetBasePosition();
            }
        }
    }

    private void Update() {
        if (handbreak) {
            HandbreakVehicle();
            return;
        }

        if (_turnDirection != default) {
            DriveVehicle();
        }
    }

    private void HandbreakVehicle() {
        _vehicle.ApplyForce(-_vehicle.Velocity, "Break", Color.black);
    }

    private void DriveVehicle() {
        var stopForce = _vehicle.StopOnFirstWall(lookaheadDistance, wallsLayerMask);
        if (stopForce != default) {
            _vehicle.ApplyForce(stopForce, "WallStop", Color.red);
        }

        var followDirectionForce = _vehicle.FollowDirection(_turnAnchor, _turnDirection, lookaheadDistance);
        _vehicle.ApplyForce(followDirectionForce, "FollowDirection", Color.green);
    }

    private Vector3 GetBaseDirection(Vector3 direction) {
        if (!snapAnchorPosition) {
            return direction;
        }

        var forwardDot = Vector3.Dot(Vector3.forward, direction);
        var rightDot = Vector3.Dot(Vector3.right, direction);
        return Mathf.Abs(forwardDot) > Mathf.Abs(rightDot) ? Vector3.forward * Mathf.Sign(forwardDot) : Vector3.right * Mathf.Sign(rightDot);
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
}
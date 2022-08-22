using Unity.VisualScripting;
using UnityEngine;

public class WheeledVehicleDriver : MonoBehaviour {
    
    [SerializeField] private bool snapAnchorPosition = true;
    [SerializeField] private bool reanchorEachInput = false;
    [SerializeField][Range(0, 1f)] private float turnBaseOffset;
    [SerializeField] private Transform anchorCheckPoint;

    private VehicleAnchoredDirectionSteering _andchoredDirectionSteering;

    public bool IsHandbreak => _andchoredDirectionSteering.IsHandbreak;

    private void Awake() {
        _andchoredDirectionSteering = GetComponent<VehicleAnchoredDirectionSteering>();
    }

    public void SetHandbreak(bool handbreak) {
        _andchoredDirectionSteering.SetHandbreak(handbreak);
    }

    public void SetSteerDirection(Vector3 direction) {
        if (direction.sqrMagnitude > 0) {
            if (reanchorEachInput || direction != _andchoredDirectionSteering.TurnDirection) {
                _andchoredDirectionSteering.SetDirection(GetBaseDirection(direction), GetBasePosition());
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
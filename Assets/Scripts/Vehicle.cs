using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[SelectionBase]
public class Vehicle : MonoBehaviour {
    [SerializeField] private float maxForce = 1;
    [SerializeField] private float maxSpeedRaw = 1;
    [SerializeField] private float mass = 1;
    [SerializeField] private float minMoveVelocity = 0.1f;

    private Vector3 _velocity;
    private Vector3 _steeringForce;
    [SerializeField] private float maxSpeedMultiplier = 1;
    private PhysicStability physicStability;

    public Vector3 Velocity => _velocity;
    public float MaxSpeed => maxSpeedRaw * maxSpeedMultiplier;
    public float MaxForce => maxForce;

    private void Awake() {
        physicStability = GetComponent<PhysicStability>();
    }

    public Vector3 PredictPosition(float futureTimeAmount) {
        return transform.position + _velocity * futureTimeAmount;
    }

    struct ForceDebugInfo {
        public string source;
        public Vector3 force;
        public Color color;
    }

    private Dictionary<string, ForceDebugInfo> debugInfoList = new Dictionary<string, ForceDebugInfo>();
    private List<ForceDebugInfo> appliedInfo = new List<ForceDebugInfo>();
    private Vector3 steeringDeltaFrameDebug;
    private Vector3 velocityBeforeChangeFrameDebug;

    public void ApplyForce(Vector3 force, string source = null, Color color = default) {
        // Debug.DrawLine(
        //     transform.position + _velocity + _steeringForce + Vector3.up * _steeringForce.magnitude,
        //     transform.position + _velocity + _steeringForce + force + Vector3.up * _steeringForce.magnitude,
        //     color
        // );
        _steeringForce += force;

        if (source == null) {
            source = "Unknown" + debugInfoList.Count;
        }

        debugInfoList[source] = new ForceDebugInfo {
            source = source,
            force = force,
            color = color
        };
    }

    public void ChangeMaxSpeedMultiplier(float multiplier) {
        maxSpeedMultiplier = multiplier;
    }

    private void Update() {
        if (physicStability != null && !physicStability.IsStable) {
            return;
        }

        _steeringForce = Vector3.ClampMagnitude(_steeringForce, maxForce);
        var steeringForceOverMass = (_steeringForce / mass) * Time.deltaTime;
        steeringDeltaFrameDebug = steeringForceOverMass;
        _steeringForce = Vector3.zero;

        appliedInfo.Clear();
        appliedInfo.AddRange(debugInfoList.Values);
        debugInfoList.Clear();
    
        velocityBeforeChangeFrameDebug = _velocity;
        _velocity += steeringForceOverMass;
        _velocity = Vector3.ClampMagnitude(_velocity, MaxSpeed);

        if (_velocity.magnitude < minMoveVelocity) {
            return;
        }
        
        var thisTransform = transform;
        var newPosition = thisTransform.position + _velocity * Time.deltaTime;
        if (newPosition.magnitude > 0) {
            thisTransform.rotation = Quaternion.LookRotation(_velocity, Vector3.up);
            thisTransform.position = newPosition;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + velocityBeforeChangeFrameDebug);
        Handles.color = Color.white;
        Handles.DrawWireDisc(transform.position, Vector3.up, maxSpeedRaw);
        Handles.DrawSolidDisc(transform.position, Vector3.up, 0.01f);
        Handles.Label(transform.position, "V: " + _velocity.magnitude.ToString("F2"));
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position + velocityBeforeChangeFrameDebug + Vector3.down * 0.05f, transform.position + velocityBeforeChangeFrameDebug + steeringDeltaFrameDebug + Vector3.down * 0.05f);
        Handles.color = Color.black;
        Handles.DrawWireDisc(transform.position + velocityBeforeChangeFrameDebug + Vector3.down * 0.05f, Vector3.up, maxForce);
        Handles.DrawSolidDisc(transform.position + velocityBeforeChangeFrameDebug + Vector3.down * 0.05f, Vector3.up, 0.01f);
        Handles.Label(transform.position + velocityBeforeChangeFrameDebug + steeringDeltaFrameDebug + Vector3.down * 0.05f, "S: " + steeringDeltaFrameDebug.magnitude.ToString("F2"));

        int heightStack = 1;
        Vector3 horizontalForceStack = Vector3.zero;
        float segmentHeight = 0.1f;
        foreach (var info in appliedInfo) {
            Vector3 upVector = Vector3.up * segmentHeight * heightStack;
            var segmentStartPosition = transform.position + velocityBeforeChangeFrameDebug + upVector + horizontalForceStack;
            var segmentEndPosition = segmentStartPosition + info.force;

            Handles.color = info.color;
            var labelStyle = new GUIStyle();
            labelStyle.normal.textColor = info.color;
            Handles.Label(segmentStartPosition, info.source, labelStyle);
            Handles.DrawLine(segmentStartPosition, segmentEndPosition);
            Handles.DrawSolidDisc(segmentStartPosition, Vector3.up, 0.01f);

            horizontalForceStack += info.force;
            heightStack += 1;
        }
    }
}
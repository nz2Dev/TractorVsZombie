using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TwoAxisMovePlatform : MonoBehaviour {

    [SerializeField] private Transform backAlignmentAxis;
    [SerializeField] private Transform frontTurnAxis;
    [SerializeField] private LayerMask wallsLayerMask;
    [SerializeField] private float wallsCheckDistance = 0.3f;
    
    private float _platformDistance;
    private float _frontAxisOffset;

    public bool customExecution = false;
    public Vector3 FrontPosition => frontTurnAxis.position;
    public Vector3 FrontForward => frontTurnAxis.forward;

    private void Awake() {
        _frontAxisOffset = frontTurnAxis.localPosition.magnitude;
        var backAxisOffset = backAlignmentAxis.localPosition.magnitude;
        _platformDistance = backAxisOffset + _frontAxisOffset;
    }

    public void SolveNow() {
        SolveAxis();
    }

    private void Update() {
        if (!customExecution) {
            SolveAxis();
        }
    }

    public void MoveBy(Vector3 vector) {
        var previousPosition = frontTurnAxis.position;
        var safeVector = vector;
        if (wallsLayerMask != 0) {
            Debug.DrawRay(frontTurnAxis.position, vector.normalized * wallsCheckDistance, Color.red);
            // todo add better gismos
            // todo Use overlap method, and take all colliders into account, otherwise when two or more, adjustments only apply to first collider hit by spherecast
            if (Physics.SphereCast(frontTurnAxis.position, wallsCheckDistance, vector.normalized, out var hit, wallsCheckDistance, wallsLayerMask)) {
                Debug.DrawRay(frontTurnAxis.position, safeVector, Color.blue, 0.1f);
                safeVector = Vector3.ProjectOnPlane(vector, Vector3.ProjectOnPlane(hit.normal, Vector3.up).normalized);
            }

            // todo cast sphere from back axis, and push out from collider in Update methods
        }

        frontTurnAxis.Translate(safeVector, Space.World);
        if (vector.magnitude > 0) {
            frontTurnAxis.rotation = Quaternion.LookRotation(vector.normalized, Vector3.up);
        }
    }

    public void MoveTo(Vector3 position) {
        var difference = position - frontTurnAxis.position;
        MoveBy(difference);
    }

    private void SolveAxis() {
        var turnPosition = frontTurnAxis.position;
        var turnRotation = frontTurnAxis.rotation;
        var alignDirection = (backAlignmentAxis.position - turnPosition).normalized;
        var alignmentPosition = frontTurnAxis.position + alignDirection * _platformDistance;
        var centerPosition = frontTurnAxis.position + alignDirection * _frontAxisOffset;
        
        transform.position = centerPosition;
        transform.LookAt(turnPosition, Vector3.up);
        frontTurnAxis.position = turnPosition;
        frontTurnAxis.rotation = turnRotation;
        backAlignmentAxis.position = alignmentPosition;
    }
    
}

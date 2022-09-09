using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class TwoAxisMovePlatform : MonoBehaviour {

    [SerializeField] private Transform backAlignmentAxis;
    [SerializeField] private Transform frontTurnAxis;
    [SerializeField] private LayerMask wallsLayerMask;
    [SerializeField] private float wallsCheckDistance = 0.3f;
    [SerializeField] private float wallsCheckRadius = 0.3f;

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
            if (SequentialAdjustments(vector, out var adjustedVector)) {
                safeVector = adjustedVector;
            }

            // todo cast sphere from back axis, and push out from collider in Update methods
        }

        frontTurnAxis.Translate(safeVector, Space.World);
        if (vector.magnitude > 0) {
            frontTurnAxis.rotation = Quaternion.LookRotation(vector.normalized, Vector3.up);
        }
    }

    private bool SequentialAdjustments(Vector3 vector, out Vector3 adjustedVector) {
        vector = Vector3.ProjectOnPlane(vector, Vector3.up);
        // Debug.DrawRay(frontTurnAxis.position, vector.normalized * wallsCheckDistance, Color.green, 0.1f);

        if (Physics.SphereCast(frontTurnAxis.position, wallsCheckRadius, vector.normalized, out var hit, wallsCheckDistance, wallsLayerMask)) {
            var firstFacingWall = hit.collider;
            var firstFacingWallHitNormal = hit.normal;
            var firstFacingWallRefraction = Vector3.ProjectOnPlane(vector, Vector3.ProjectOnPlane(firstFacingWallHitNormal, Vector3.up).normalized);
            var hitNewWall = false;
            
            // Debug.DrawRay(hit.point + hit.normal * 0.1f, hit.normal, Color.red);
            // Debug.DrawRay(hit.point + hit.normal * 0.1f, firstFacingWallRefraction.normalized * 0.3f, Color.red);

            do {
                adjustedVector = firstFacingWallRefraction;
                if (Physics.SphereCast(frontTurnAxis.position, wallsCheckRadius, firstFacingWallRefraction.normalized, out var nextWallHit, wallsCheckDistance, wallsLayerMask)) {
                    var nextWall = nextWallHit.collider;
                    var nextWallHitNormal = nextWallHit.normal;
                    var nextWallRefraction = Vector3.ProjectOnPlane(firstFacingWallRefraction, Vector3.ProjectOnPlane(nextWallHitNormal, Vector3.up).normalized);

                    // Debug.DrawRay(nextWallHit.point + nextWallHit.normal * 0.1f, nextWallHit.normal, Color.blue);
                    // Debug.DrawRay(nextWallHit.point + nextWallHit.normal * 0.1f, nextWallRefraction.normalized * 0.3f, Color.blue);

                    if (Vector3.Dot(nextWallHitNormal, firstFacingWallHitNormal) > 0) {
                        var vectorOnWallRefraction = Vector3.ProjectOnPlane(vector, Vector3.ProjectOnPlane(nextWallHitNormal, Vector3.up).normalized);
                        if (Vector3.Dot(firstFacingWallRefraction, vectorOnWallRefraction) < 0) {
                            adjustedVector = Vector3.zero;
                            return true;
                        }
                    } else {
                        if (Vector3.Dot(firstFacingWallRefraction, nextWallRefraction) > 0) {
                            adjustedVector = Vector3.zero;
                            return true;
                        }
                    }

                    firstFacingWall = nextWall;
                    firstFacingWallHitNormal = nextWallHitNormal;
                    firstFacingWallRefraction = nextWallRefraction;
                    hitNewWall = true;
                }

                hitNewWall = false;
            } while (hitNewWall);

            return true;
        }

        adjustedVector = vector;
        return false;
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

    private void OnDrawGizmosSelected() {
        Handles.color = Color.green;
        Handles.DrawWireDisc(frontTurnAxis.position, Vector3.up, wallsCheckRadius);
        Handles.DrawWireDisc(frontTurnAxis.position, Vector3.up, wallsCheckRadius + wallsCheckDistance);
    }

}

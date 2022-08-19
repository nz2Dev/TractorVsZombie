using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoAxisMovePlatform : MonoBehaviour {

    [SerializeField] private Transform backAlignmentAxis;
    [SerializeField] private Transform frontTurnAxis;
    private float _platformDistance;
    private float _frontAxisOffset;

    public bool customExecution = false;
    public Transform TurnAxis => frontTurnAxis;

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

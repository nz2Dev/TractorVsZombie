using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoAxisVehicle : MonoBehaviour {

    [SerializeField] private float turnDegreeMin = -45;
    [SerializeField] private float turnDegreeMax = 45;
    [SerializeField] private Transform backAlignmentAxis;
    [SerializeField] private Transform frontTurnAxis;
    private float _platformDistance;
    private float _frontAxisOffset;

    public Transform TurnAxis => frontTurnAxis;

    private void Awake() {
        _frontAxisOffset = frontTurnAxis.localPosition.magnitude;
        var backAxisOffset = backAlignmentAxis.localPosition.magnitude;
        _platformDistance = backAxisOffset + _frontAxisOffset;

        var rotationLimiter = frontTurnAxis.GetComponent<AngleRotationLimiter>();
        if (rotationLimiter != null) {
            rotationLimiter.yDegreeMin = turnDegreeMin;
            rotationLimiter.yDegreeMax = turnDegreeMax;
        }
    }

    private void Update() {
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

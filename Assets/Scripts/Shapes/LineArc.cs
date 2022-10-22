using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LineArc : MonoBehaviour {

    [SerializeField] private AnimationCurve bendCurve;
    [SerializeField] private int segmentsCount = 5;
    [SerializeField] private float height = 5f;

    private LineRenderer _lineRenderer;

    private void Awake() {
        _lineRenderer = GetComponentInChildren<LineRenderer>(true);
        _lineRenderer.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
        _lineRenderer.alignment = LineAlignment.TransformZ;
        _lineRenderer.textureMode = LineTextureMode.Tile;
        _lineRenderer.useWorldSpace = true;
    }

    public void UpdateArc(Vector3 start, Vector3 end) {
        var startToEnd = end - start;
        var segmentLength = startToEnd / segmentsCount;
        var segmentLengthNormalized = 1.0f / segmentsCount;

        if (_lineRenderer.positionCount != segmentsCount + 1) {
            _lineRenderer.positionCount = segmentsCount + 1;
        }
        for (int segment = 0; segment < segmentsCount + 1; segment++) {
            var position = start + segmentLength * segment + Vector3.up * bendCurve.Evaluate(segmentLengthNormalized * segment);
            _lineRenderer.SetPosition(segment, position);
        }
    }
    
}

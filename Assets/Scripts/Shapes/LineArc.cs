using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LineArc : MonoBehaviour {

    [SerializeField] private AnimationCurve bendCurve;
    [SerializeField] private int segmentsCount = 5;
    [SerializeField] private float height = 5f;
    [SerializeField] private float previewLength = 2.0f;

    private LineRenderer _lineRenderer;

    private void Awake() {
        _lineRenderer = GetComponentInChildren<LineRenderer>(true);
        _lineRenderer.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
        _lineRenderer.textureMode = LineTextureMode.Stretch;
        _lineRenderer.useWorldSpace = true;
    }

    private void OnValidate() {
        if (_lineRenderer != null) {
            UpdateArc(transform.position, transform.position + transform.right * previewLength);
        }
    }

    public void FillArc(float fillAmount) {
        var material = Application.isPlaying ? _lineRenderer.material : _lineRenderer.sharedMaterial;
        material.SetFloat("_FillAmount", fillAmount);
        material.SetFloat("_SubstractAmount", 0.0f);
    }

    public void SubstractArc(float substractAmount) {
        var material = Application.isPlaying ? _lineRenderer.material : _lineRenderer.sharedMaterial;
        material.SetFloat("_FillAmount", 1.0f);
        material.SetFloat("_SubtractAmount", substractAmount);
    }

    public void UpdateArc(Vector3 start, Vector3 end) {
        var startToEnd = end - start;
        var segmentLength = startToEnd / segmentsCount;
        var segmentLengthNormalized = 1.0f / segmentsCount;

        if (_lineRenderer.positionCount != segmentsCount + 1) {
            _lineRenderer.positionCount = segmentsCount + 1;
        }

        var p0 = start;
        var arcLength = 0f;
        for (int segment = 0; segment < segmentsCount + 1; segment++) {
            var position = start + segmentLength * segment + Vector3.up * bendCurve.Evaluate(segmentLengthNormalized * segment) * height;
            _lineRenderer.SetPosition(segment, position);
            
            arcLength += Vector3.Distance(position, p0);
            p0 = position;
        }

        var material = Application.isPlaying ? _lineRenderer.material : _lineRenderer.sharedMaterial;
        material.SetFloat("_PatternLength", arcLength);
    }
    
}

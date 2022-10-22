using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class TargetOutline : MonoBehaviour {

    [SerializeField] private float scalingTime = 0.25f;
    [SerializeField] private float yOffset = 0.05f;

    private Circle _circle;
    private LineArc _trajectory;

    private Coroutine _scalingCoroutine;

    private void Awake() {
        _circle = GetComponentInChildren<Circle>(true);
        _trajectory = GetComponentInChildren<LineArc>(true);
    }

    private void Start() {
        _circle.gameObject.SetActive(false);
        _trajectory.gameObject.SetActive(false);
    }

    public void StartOutlining(Vector3 anchor, Vector3 target, float radius) {
        _circle.gameObject.SetActive(true);
        _trajectory.gameObject.SetActive(true);

        _scalingCoroutine = StartCoroutine(RadiusScalingRoutine(radius));
        
        OutlineTarget(anchor, target);
    }

    private IEnumerator RadiusScalingRoutine(float radius) {
        var scalingProgressTime = 0.0f;
        var initialRadius = radius * 0.2f;
        
        while (scalingProgressTime < scalingTime) {
            var currentRadius = initialRadius + (scalingProgressTime / scalingTime) * (radius - initialRadius);
            _circle.SetRadius(currentRadius);

            scalingProgressTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        _circle.SetRadius(radius);
    }

    public void OutlineTarget(Vector3 anchor, Vector3 target) {
        _trajectory.UpdateArc(anchor, target);
        _circle.transform.position = target + Vector3.up * yOffset;
    }

    public void StopOutlining() {
        _circle.gameObject.SetActive(false);
        _trajectory.gameObject.SetActive(false);
        
        if (_scalingCoroutine != null) {
            StopCoroutine(_scalingCoroutine);
            _scalingCoroutine = null;
        }
    }
    
}

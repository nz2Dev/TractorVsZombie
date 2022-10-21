using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TargetOutline : MonoBehaviour {

    [SerializeField] private Circle circle;
    [SerializeField] private float scalingTime = 0.25f;
    [SerializeField] private float yOffset = 0.05f;
    [SerializeField] private bool preview = false;

    private Coroutine _scalingCoroutine;

    private void Start() {
        circle.gameObject.SetActive(false);
    }

    public void StartOutlining(Vector3 point, float radius) {
        circle.gameObject.SetActive(true);
        _scalingCoroutine = StartCoroutine(RadiusScalingRoutine(radius));
        OutlinePoint(point);
    }

    private IEnumerator RadiusScalingRoutine(float radius) {
        var scalingProgressTime = 0.0f;
        var initialRadius = radius * 0.2f;
        
        while (scalingProgressTime < scalingTime) {
            var currentRadius = initialRadius + (scalingProgressTime / scalingTime) * (radius - initialRadius);
            circle.SetRadius(currentRadius);

            scalingProgressTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        circle.SetRadius(radius);
    }

    public void OutlinePoint(Vector3 point) {
        circle.transform.position = point + Vector3.up * yOffset;
    }

    public void StopOutlining() {
        circle.gameObject.SetActive(false);
        
        if (_scalingCoroutine != null) {
            StopCoroutine(_scalingCoroutine);
            _scalingCoroutine = null;
        }
    }

    private void OnValidate() {
        circle.gameObject.SetActive(preview);
    }
    
}

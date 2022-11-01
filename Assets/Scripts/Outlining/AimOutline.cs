using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class AimOutline : MonoBehaviour {

    [SerializeField] private float scalingTime = 0.25f;
    [SerializeField] private float yOffset = 0.05f;

    private Circle _circle;
    private LineArc _trajectory;
    private Coroutine _activationCoroutine;

    private void Awake() {
        _circle = GetComponentInChildren<Circle>(true);
        _circle.gameObject.SetActive(false);

        _trajectory = GetComponentInChildren<LineArc>(true);
        _trajectory.gameObject.SetActive(false);
    }    

    public void StartOutlining(Vector3 anchor, Vector3 target, float radius) {
        _circle.gameObject.SetActive(true);
        _trajectory.gameObject.SetActive(true);

        if (_activationCoroutine != null) {
            StopCoroutine(_activationCoroutine);
        }
        _activationCoroutine = StartCoroutine(ActivationAnimationRoutine(radius));
        
        OutlineTarget(anchor, target);
    }

    private IEnumerator ActivationAnimationRoutine(float radius) {
        var scalingProgressTime = 0.0f;
        var initialRadius = radius * 0.2f;
        
        while (scalingProgressTime < scalingTime) {
            var currentRadius = initialRadius + (scalingProgressTime / scalingTime) * (radius - initialRadius);
            _circle.SetRadius(currentRadius);
            _trajectory.FillArc(scalingProgressTime);

            scalingProgressTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        _circle.SetRadius(radius);
        _trajectory.FillArc(1.0f);
    }

    public void OutlineTarget(Vector3 anchor, Vector3 target) {
        _trajectory.UpdateArc(anchor, target);
        _circle.transform.position = target + Vector3.up * yOffset;
    }

    public void StopOutlining() {
        _circle.gameObject.SetActive(false);
        _trajectory.gameObject.SetActive(false);
        
        if (_activationCoroutine != null) {
            StopCoroutine(_activationCoroutine);
            _activationCoroutine = null;
        }
    }
    
}

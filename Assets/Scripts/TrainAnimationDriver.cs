using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TrainAnimationDriver : MonoBehaviour {
    [SerializeField] private AnimationCurve curveTurnX;
    [SerializeField] private AnimationCurve curveTurnY;
    [SerializeField] private AnimationCurve curveTurnRotation;
    [SerializeField] private float stepSize = 1;

    private bool right;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            StartCoroutine(Turning(right, Time.time));
        }

        if (Input.GetKeyDown(KeyCode.LeftControl)) {
            StartCoroutine(nameof(TurningLoop));
        }

        if (Input.GetKeyDown(KeyCode.Tab)) {
            right = !right;
        }
    }

    private IEnumerator TurningLoop() {
        var stepInterval = 1f;
        for (int i = 0; i < 4; i++) {
            yield return Turning(right, Time.time, stepInterval);
        }
    }

    private IEnumerator Turning(bool right, float startTime, float duration = 1f) {
        var trainTransform = transform;
        var startPosition = trainTransform.position;
        var startRotationY = trainTransform.rotation.eulerAngles.y;
        var startLocalToWorldMatrix = trainTransform.localToWorldMatrix;
        var turnDirection = trainTransform.TransformDirection(right ? Vector3.right : Vector3.left);
        var sign = right ? 1 : -1;
        var time = 0f;

        while (time < 1f) {
            time = (Time.time - startTime) / duration;
            if (time > 1) {
                time = 1;
            }
            
            trainTransform.position = startPosition + startLocalToWorldMatrix.MultiplyVector(
                new Vector3(
                    sign * curveTurnX.Evaluate(time) * stepSize,
                    0,
                    curveTurnY.Evaluate(time) * stepSize
                )
            );
            
            trainTransform.rotation = Quaternion.Euler(
                0,
                startRotationY + curveTurnRotation.Evaluate(time) * sign * 90f,
                0
            );

            yield return new WaitForEndOfFrame();
        }
    }
}
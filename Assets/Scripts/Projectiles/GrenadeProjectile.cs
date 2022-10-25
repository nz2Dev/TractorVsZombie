using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour {

    [SerializeField] private float detonateDistance = 0.3f;

    private bool _launched;

    public void Launch(AnimationCurve flyCurve, float flyHeight, Vector3 landPosition, Action<GrenadeProjectile> landCallback) {
        if (_launched) {
            return;
        }

        StartCoroutine(FlyCoroutine(flyCurve, flyHeight, landPosition, landCallback));
        _launched = true;
    }

    private IEnumerator FlyCoroutine(AnimationCurve flyCurve, float flyHeight, Vector3 landPosition, Action<GrenadeProjectile> landCallback) {
        var time = 0f;
        var launchPosition = transform.position;
        var launchToLand = landPosition - launchPosition;

        while (true) {
            time += Time.deltaTime;

            var flyPositionLocal = launchToLand * time + flyCurve.Evaluate(time) * flyHeight * Vector3.up;
            transform.position = launchPosition + flyPositionLocal;

            if (Vector3.Distance(transform.position, landPosition) < detonateDistance || time >= 1f) {
                landCallback?.Invoke(this);
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}

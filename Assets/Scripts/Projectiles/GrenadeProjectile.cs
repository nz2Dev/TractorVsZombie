using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour {

    [SerializeField] private float detonateDistance = 0.3f;
    [SerializeField] private String globalLandingObjectPopulatorName = "LandingOutlinePopulator";
    [SerializeField] private SphereExplosion explosiveness;

    private NamedPrototypePopulator _landingOutlinesPopulator;
    private bool _launched;

    private void Awake() {
        _landingOutlinesPopulator = GameObject.Find(globalLandingObjectPopulatorName).GetComponent<NamedPrototypePopulator>();
    }

    public void Launch(AnimationCurve flyCurve, float flyHeight, Vector3 landPosition, Action<Vector3, RaycastHit[]> landCallback) {
        if (_launched) {
            return;
        }

        StartCoroutine(FlyCoroutine(flyCurve, flyHeight, landPosition, landCallback));
        _launched = true;
    }

    private IEnumerator FlyCoroutine(AnimationCurve flyCurve, float flyHeight, Vector3 landPosition, Action<Vector3, RaycastHit[]> landCallback) {
        var time = 0f;
        var launchPosition = transform.position;
        var launchToLand = landPosition - launchPosition;

        var landingOutline = _landingOutlinesPopulator.GetOrCreateChild<LandingOutline>(gameObject.GetInstanceID());
        landingOutline.OutlineLanding(landPosition);

        while (true) {
            time += Time.deltaTime;

            var flyPositionLocal = launchToLand * time + flyCurve.Evaluate(time) * flyHeight * Vector3.up;
            transform.position = launchPosition + flyPositionLocal;

            var distanceToLanding = Vector3.Distance(transform.position, landPosition);
            landingOutline.SetProgress(1 - (distanceToLanding / launchToLand.magnitude));

            if (distanceToLanding < detonateDistance || time >= 1f) {
                _landingOutlinesPopulator.DestroyChild(gameObject.GetInstanceID());
                explosiveness.Explode(landCallback);
                Destroy(gameObject);
                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }
}

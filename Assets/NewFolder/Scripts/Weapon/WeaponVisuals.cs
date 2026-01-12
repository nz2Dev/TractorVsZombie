using System;

using UnityEngine;

public class WeaponVisuals : MonoBehaviour {
    
    [SerializeField] private ParticleSystem cannoParticlesSystem;
    [SerializeField] private Transform rotationBase;

    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    internal void UpdatePosition(Vector3 position) {
        transform.position = position;
    }

    internal void UpdateAimPoint(Vector3 aimPoint, BallisticConfig config) {
        switch (config.type) {
            case BallisticType.Bullet:
                RotateGunAim(aimPoint);
                break;
            case BallisticType.Rocket:
                RotateRocketAim(aimPoint, config.rocket);
                break;
        }
    }

    private void RotateRocketAim(Vector3 aimPoint, RocketConfig config) {
        var flyTangent = GetTangent(transform.position, aimPoint, config.flyCurve, config.amplitude, 0);
        rotationBase.rotation = Quaternion.LookRotation(flyTangent, Vector3.up);
    }

    private void RotateGunAim(Vector3 aimPoint) {
        var aimForward = (aimPoint - transform.position).normalized;
        var aimForwardFlat = Vector3.ProjectOnPlane(aimForward, Vector3.up);
        rotationBase.rotation = Quaternion.LookRotation(aimForwardFlat, Vector3.up);
    }

    internal void ShowActivation(BallisticType ballisticType) {
        if (ballisticType == BallisticType.Bullet) {
            ShowGunFire();
        }
    }

    private void ShowGunFire() {
        animator.SetTrigger("Fire");
        cannoParticlesSystem.Emit(1);
    }

    Vector3 GetPointOnCurve(Vector3 start, Vector3 end, AnimationCurve curve, float curveScale, float t) {
        Vector3 horizontal = Vector3.Lerp(start, end, t);
        float height = curve.Evaluate(t) * curveScale;
        return new Vector3(horizontal.x, horizontal.y + height, horizontal.z);
    }

    Vector3 GetTangent(Vector3 start, Vector3 end, AnimationCurve curve, float curveScale, float t) {
        float delta = 0.01f;
        Vector3 p1 = GetPointOnCurve(start, end, curve, curveScale, t);
        Vector3 p2 = GetPointOnCurve(start, end, curve, curveScale, Mathf.Min(t + delta, 1f));
        return (p2 - p1).normalized;
    }

}
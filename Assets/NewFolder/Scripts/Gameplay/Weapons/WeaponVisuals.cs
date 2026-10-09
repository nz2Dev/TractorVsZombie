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

    internal void UpdateAimPoint(Vector3 aimPoint, BallisticPrototype ballisticPrototype) {
        switch (ballisticPrototype.type) {
            case BallisticType.Bullet:
                RotateGunAim(aimPoint);
                break;
            case BallisticType.Rocket:
                RotateRocketAim(aimPoint, ballisticPrototype.rocketPrototype.config.flyShape);
                break;
        }
    }

    private void RotateRocketAim(Vector3 aimPoint, FlyShape flyShape) {
        var flyTangent = flyShape.GetTangent(transform.position, aimPoint, 0);
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

}
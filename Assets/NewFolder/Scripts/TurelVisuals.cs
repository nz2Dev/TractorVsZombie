using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;

public class TurelVisuals : MonoBehaviour {
    
    [SerializeField] private ParticleSystem cannoParticlesSystem;
    [SerializeField] private ParticleSystem bulletSystem;

    private Animator animator;

    private ParticleSystem.Particle[] bulletParticles;
    private List<Vector4> customData;
    private int _activeCount;

    private void Awake() {
        animator = GetComponent<Animator>();
        bulletParticles = new ParticleSystem.Particle[bulletSystem.main.maxParticles];
        customData = new List<Vector4>(bulletSystem.main.maxParticles);
    }

    public void UpdatePosition(Vector3 position) {
        transform.position = position;
    }

    public void UpdateAim(Vector3 aimForward) {
        var aimForwardFlat = Vector3.ProjectOnPlane(aimForward, Vector3.up);
        transform.rotation = Quaternion.LookRotation(aimForwardFlat, Vector3.up);
    }

    public void ShowBulletFire(int shootId, Vector3 velocity) {
        animator.SetTrigger("Fire");
        EmitBulletParticle(shootId, velocity);
        EmitCannoParticle();
    }

    private void EmitCannoParticle() {
        cannoParticlesSystem.Emit(1);
    }

    public void KillBulletFire(int shootId) {
        KillBuletParticleById(shootId);
    }

    public void EmitBulletParticle(int id, Vector3 velocity) {
        var emitParams = new ParticleSystem.EmitParams { velocity = velocity };
        bulletSystem.Emit(emitParams, 1);

        var count = bulletSystem.GetParticles(bulletParticles);
        bulletSystem.GetCustomParticleData(customData, ParticleSystemCustomData.Custom1);
        customData[count - 1] = new Vector4(id, 0, 0, 0);
        bulletSystem.SetCustomParticleData(customData, ParticleSystemCustomData.Custom1);
    }

    public void KillBuletParticleById(int shootId) {
        var count = bulletSystem.GetParticles(bulletParticles);
        bulletSystem.GetCustomParticleData(customData, ParticleSystemCustomData.Custom1);

        for (int i = 0; i < count; i++) {
            int id = (int) customData[i].x;
            if (id == shootId) {
                bulletParticles[i].remainingLifetime = -1f;
                break;
            }
        }

        bulletSystem.SetParticles(bulletParticles, count);
    }

}
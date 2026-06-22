using System;
using System.Collections.Generic;

using UnityEngine;

public class ProjectileView {

    private readonly SoundManager soundManager;
    private ParticleSystem bulletSystem;
    private ParticleSystem.Particle[] bulletParticles;
    private List<Vector4> customData;

    public ProjectileView(SoundManager soundManager) {
        var prefab = Resources.Load<ParticleSystem>("Projectile ParticlesSystem");
        bulletSystem = GameObject.Instantiate(prefab);
        bulletParticles = new ParticleSystem.Particle[bulletSystem.main.maxParticles];
        customData = new List<Vector4>(bulletSystem.main.maxParticles);
        this.soundManager = soundManager;
    }

    internal void ShowBulletShoot(int projectileId, Vector3 position, Vector3 velocity, ProjectileStyle style, AudioClip[] shootSFX) {
        EmitBulletParticle(projectileId, position, velocity, style);
        soundManager.PlayEffect(position, shootSFX);
    }

    internal void ShowBulletCrash(int projectileId, Vector3 position, AudioClip[] impactSFX, ParticleSystem impactParticlesPrefab, Vector3 hitDirection) {
        KillBuletParticleById(projectileId);
        if (impactSFX != null)
            soundManager.PlayEffect(position, impactSFX);
        if (impactParticlesPrefab != null)
            GameObject.Instantiate(impactParticlesPrefab, position, Quaternion.LookRotation(hitDirection));
    }

    internal void ShowBulletDisappear(int projectileId) {
        KillBuletParticleById(projectileId);
    }

    private void EmitBulletParticle(int id, Vector3 position, Vector3 velocity, ProjectileStyle style) {
        var emitParams = new ParticleSystem.EmitParams { 
            position = position,
            velocity = velocity,
            startColor = style.startColor,
            startSize = style.startSize
        };
        bulletSystem.Emit(emitParams, 1);

        var count = bulletSystem.GetParticles(bulletParticles);
        bulletSystem.GetCustomParticleData(customData, ParticleSystemCustomData.Custom1);
        customData[count - 1] = new Vector4(id, 0, 0, 0);
        bulletSystem.SetCustomParticleData(customData, ParticleSystemCustomData.Custom1);
    }

    private void KillBuletParticleById(int shootId) {
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
using System;
using System.Collections.Generic;

using UnityEngine;

public class ProjectileView {

    private readonly SoundManager soundManager;
    private ParticleSystem bulletSystem;
    private ParticleSystem.Particle[] bulletParticles;
    private List<Vector4> customData;

    private Dictionary<int, AudioSource> shooterShootRegistry = new();
    private Dictionary<int, AudioSource> shooterCrashRegistry = new();

    public ProjectileView(SoundManager soundManager) {
        var prefab = Resources.Load<ParticleSystem>("Projectile ParticlesSystem");
        bulletSystem = GameObject.Instantiate(prefab);
        bulletParticles = new ParticleSystem.Particle[bulletSystem.main.maxParticles];
        customData = new List<Vector4>(bulletSystem.main.maxParticles);
        this.soundManager = soundManager;
    }

    internal void SetupShooter(int shooterId, AudioSource shootAudioSourcePrefab, AudioSource crashAudioSourcePrefab) {
        if (shooterShootRegistry.ContainsKey(shooterId))
            return;
        
        shooterShootRegistry[shooterId] = GameObject.Instantiate(shootAudioSourcePrefab);
        shooterCrashRegistry[shooterId] = GameObject.Instantiate(crashAudioSourcePrefab);
    }

    internal void ShowBulletShoot(int shooterId, int projectileId, Vector3 position, Vector3 velocity, ProjectileStyle style, AudioClip[] shootSFX) {
        EmitBulletParticle(projectileId, position, velocity, style);
        
        var audioSource = shooterShootRegistry[shooterId];
        audioSource.transform.position = position;
        audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.4f);
        audioSource.PlayOneShot(SoundManager.SelectRandom(shootSFX));
    }

    internal void ShowBulletCrash(int shooterId, int projectileId, Vector3 position, AudioClip[] impactSFX, ParticleSystem impactParticlesPrefab, Vector3 hitDirection) {
        KillBuletParticleById(projectileId);
        
        if (impactSFX != null) {
            var audioSource = shooterCrashRegistry[shooterId];
            audioSource.transform.position = position;
            audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.4f);
            audioSource.PlayOneShot(SoundManager.SelectRandom(impactSFX));
        }

        if (impactParticlesPrefab != null) {
            GameObject.Instantiate(impactParticlesPrefab, position, Quaternion.LookRotation(hitDirection));
        }
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
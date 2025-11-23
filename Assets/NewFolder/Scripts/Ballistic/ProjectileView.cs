using System;
using System.Collections.Generic;

using UnityEngine;

public class ProjectileView : MonoBehaviour {

    [SerializeField] private ParticleSystem bulletSystemPrefab;

    private ParticleSystem bulletSystem;
    private ParticleSystem.Particle[] bulletParticles;
    private List<Vector4> customData;

    internal void Start() {
        bulletSystem = GameObject.Instantiate(bulletSystemPrefab);
        bulletParticles = new ParticleSystem.Particle[bulletSystem.main.maxParticles];
        customData = new List<Vector4>(bulletSystem.main.maxParticles);
    }

    internal void ShowBulletShoot(int projectileId, Vector3 position, Vector3 velocity) {
        EmitBulletParticle(projectileId, position, velocity);
    }

    internal void ShowBulletCrash(int projectileId) {
        KillBuletParticleById(projectileId);
    }

    internal void ShowBulletDisappear(int projectileId) {
        KillBuletParticleById(projectileId);
    }

    private void EmitBulletParticle(int id, Vector3 position, Vector3 velocity) {
        var emitParams = new ParticleSystem.EmitParams { 
            position = position,
            velocity = velocity 
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
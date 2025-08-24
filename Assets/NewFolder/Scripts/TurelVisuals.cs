using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;

public class TurelVisuals : MonoBehaviour {
    
    [SerializeField] private ParticleSystem cannoParticlesSystem;
    [SerializeField] private ParticleSystem bulletSystem;

    private Animator animator;

    private ParticleSystem.Particle[] bulletParticles;
    private int _activeCount;

    private void Awake() {
        animator = GetComponent<Animator>();
        bulletParticles = new ParticleSystem.Particle[bulletSystem.main.maxParticles];
    }

    public void UpdatePosition(Vector3 position) {
        transform.position = position;
    }

    public void UpdateAim(Vector3 aimForward) {
        transform.rotation = Quaternion.LookRotation(aimForward, Vector3.up);
    }

    public void ShowShootEffect(int shootId, Vector3 velocity) {
        animator.SetTrigger("Shoot");
        EmitWithId(shootId, velocity);
    }

    public void KillShootBullet(int shootId) {
        KillParticleById(shootId);
    }

    private void EmitShootProjectile(int shootId, Vector3 velocity) {
        _activeCount++;
        bulletSystem.Emit(new ParticleSystem.EmitParams {
            velocity = velocity
        }, 1);
    }

    private List<Vector4> customData = new List<Vector4>();

    public void EmitWithId(int id, Vector3 velocity) {
        // Emit one particle
        var emitParams = new ParticleSystem.EmitParams {
            velocity = velocity
        };
        bulletSystem.Emit(emitParams, 1);

        var count = bulletSystem.GetParticles(bulletParticles);

        // Get custom data
        bulletSystem.GetCustomParticleData(customData, ParticleSystemCustomData.Custom1);
        customData[count - 1] = new Vector4(id, 0, 0, 0);

        // Write back
        bulletSystem.SetCustomParticleData(customData, ParticleSystemCustomData.Custom1);
    }

    private void DestroyShootProjectile(int index) {
        _activeCount--;

        bulletSystem.GetParticles(bulletParticles);
        bulletParticles[index].remainingLifetime = -1;
        bulletParticles[index] = bulletParticles[_activeCount];

        bulletSystem.SetParticles(bulletParticles, _activeCount);
    }

    public void KillParticleById(int targetId) {
        var count = bulletSystem.GetParticles(bulletParticles);
        bulletSystem.GetCustomParticleData(customData, ParticleSystemCustomData.Custom1);

        for (int i = 0; i < count; i++) {
            int id = (int) customData[i].x;
            if (id == targetId) {
                bulletParticles[i].remainingLifetime = -1f; // mark dead
                break;
            }
        }

        bulletSystem.SetParticles(bulletParticles, count);
    }

}
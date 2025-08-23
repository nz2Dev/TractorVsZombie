using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;

public class TurelVisuals : MonoBehaviour {
    
    [SerializeField] private ParticleSystem cannoParticlesSystem;
    [SerializeField] private ParticleSystem projectileParticlesSystem;

    private Animator animator;

    private ParticleSystem.Particle[] projectileParticles;
    private int _activeCount;

    private void Awake() {
        animator = GetComponent<Animator>();
        projectileParticles = new ParticleSystem.Particle[projectileParticlesSystem.main.maxParticles];
    }

    public void UpdatePosition(Vector3 position) {
        transform.position = position;
    }

    public void UpdateAim(Vector3 aimForward) {
        transform.rotation = Quaternion.LookRotation(aimForward, Vector3.up);
    }

    public void ShowShootEffect(int shootId, Vector3 velocity) {
        animator.SetTrigger("Shoot");
        EmitShootProjectile(shootId, velocity);
    }

    public void KillShootBullet(int bulletIndex) {
        DestroyShootProjectile(bulletIndex);
    }

    private void EmitShootProjectile(int orderNumber, Vector3 velocity) {
        bool isNextActiveCount = orderNumber == _activeCount + 1;
        Assert.IsTrue(isNextActiveCount);
        
        _activeCount++;
        projectileParticlesSystem.Emit(new ParticleSystem.EmitParams {
            velocity = velocity
        }, 1);
    }

    private void DestroyShootProjectile(int index) {
        _activeCount--;
        projectileParticles[_activeCount].remainingLifetime = -1;
        projectileParticles[index] = projectileParticles[_activeCount];

        projectileParticlesSystem.SetParticles(projectileParticles, _activeCount);
    }

}
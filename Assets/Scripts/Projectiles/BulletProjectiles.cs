using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class BulletProjectiles : MonoBehaviour {

    [SerializeField] private Collider aimTrigger;
    
    private ParticleSystem _particleSystem;
    private List<Particle> _triggeredParticles = new List<Particle>(1);

    public event Action OnAimTriggerHit;
    public event Action<GameObject> OnCollisionHit;

    private void Awake() {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    public void StartFire() {
        _particleSystem.Play();
    }

    public void SetAimTriggerPosition(Vector3 position) {
        aimTrigger.transform.position = position;
    }

    public void StopFire() {
        _particleSystem.Stop();
    }

    public void SetFireSpeed(float particlesPerSecond) {
        var emission = _particleSystem.emission;
        emission.rateOverTime = particlesPerSecond;
    }

    private void OnParticleTrigger() {
        var number = ParticlePhysicsExtensions.GetTriggerParticles(
            _particleSystem, ParticleSystemTriggerEventType.Enter, _triggeredParticles);
        
        for (int particleIndex = 0; particleIndex < number; particleIndex++) {
            OnAimTriggerHit?.Invoke();
        }
    }

    private void OnParticleCollision(GameObject other) {
        OnCollisionHit?.Invoke(other); // it's gameObject with rigidbody that the collider is attached to if any
    }

}

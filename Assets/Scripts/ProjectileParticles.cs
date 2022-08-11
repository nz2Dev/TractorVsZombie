using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileParticles : MonoBehaviour {
    
    private ParticleSystem _particleSystem;

    private void Awake() {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    public void StartFire() {
        _particleSystem.Play();
    }

    public void StopFire() {
        _particleSystem.Stop();
    }

    public void SetFireSpeed(float particlesPerSecond) {
        var emission = _particleSystem.emission;
        emission.rateOverTime = particlesPerSecond;
    }

}

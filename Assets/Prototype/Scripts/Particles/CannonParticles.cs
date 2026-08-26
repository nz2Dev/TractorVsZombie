using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonParticles : MonoBehaviour {
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

    public void SetEmitSpeed(float cannonsPerSecond) {
        var emission = _particleSystem.emission;
        emission.rateOverTime = cannonsPerSecond;
    }
}

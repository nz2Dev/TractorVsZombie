using UnityEngine;

public class RamView {
    
    private readonly SoundManager soundManager;

    public RamView(SoundManager soundManager) {
        this.soundManager = soundManager;
    }

    public void PlayImpact(Vector3 position, float radius, int times, AudioClip[] sfx) {
        for (int i = 0; i < times; i++) {
            var impactPosition = position + Random.onUnitSphere * radius;
            soundManager.PlayEffectDelayed(impactPosition, i * 0.05f, sfx);
        }
    }   

}
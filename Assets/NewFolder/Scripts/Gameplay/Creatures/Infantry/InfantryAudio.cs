using UnityEngine;

public class InfantryAudio {

    private readonly FootstepSoundSystem footstepSoundSystem;

    public InfantryAudio(FootstepSoundSystem footstepSoundSystem) {
        this.footstepSoundSystem = footstepSoundSystem;
    }

    public void RegisterFootstep(Vector3 position, float speedNormalized) {
        footstepSoundSystem.RegisterFootstep(position, speedNormalized);
    }
}

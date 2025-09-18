using System;

using UnityEngine;

public class UnitVisuals : MonoBehaviour {
    
    private Animation animations;

    private bool sheduledForDestruction;

    private void Awake() {
        animations = GetComponent<Animation>();
    }

    void LateUpdate() {
        if (sheduledForDestruction && !animations.isPlaying) {
            Destroy(gameObject);
        }
    }

    private void Start() {
        var renderer = GetComponentInChildren<Renderer>();
        renderer.material = new Material(renderer.sharedMaterial);
    }

    internal void UpdatePositionAndRotation(Vector3 position, Quaternion rotation) {
        transform.SetPositionAndRotation(position, rotation);
    }

    internal void RotateAway(Vector3 sourcePosition) {
        var sourceToPosition = transform.position - sourcePosition;
        transform.rotation = Quaternion.LookRotation(-sourceToPosition.normalized, Vector3.up);
    }

    internal void PlayDirectAttackAnimation() {
        animations.Play("Attack Animation");
        animations.PlayQueued("Walk Animation");
    }

    internal void PlayFinalBlowAnimation() {
        animations.Play("Death Animation", PlayMode.StopAll);
    }

    internal void DestroySelfOnIdle() {
        sheduledForDestruction = true;
    }
}
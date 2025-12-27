using UnityEngine;

public class BodyVisuals : MonoBehaviour {
    
    private Animator animator;

    private bool sheduledForDestruction;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    void LateUpdate() {
        if (sheduledForDestruction && !IsAnimatorPlaying()) {
            Destroy(gameObject);
        }
    }

    private bool IsAnimatorPlaying() {
        var curentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return animator.IsInTransition(0) || curentStateInfo.normalizedTime < curentStateInfo.length;
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
        sourceToPosition.y = 0;
        transform.rotation = Quaternion.LookRotation(-sourceToPosition.normalized, Vector3.up);
    }

    internal void PlayTakeHit() {
        animator.SetTrigger("Take Hit");
    }

    internal void PlayDirectAttackAnimation() {
        animator.SetTrigger("Attack");
    }

    internal void PlayPushedAwayDeathAnimation() {
        animator.SetTrigger("Throw Death");
    }

    internal void PlayDisolveAnimation() {
        animator.SetTrigger("Disolve Death");
    }

    internal void DestroySelfOnIdle() {
        sheduledForDestruction = true;
    }
}
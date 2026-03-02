using UnityEngine;

[ExecuteInEditMode]
public class InfantryVisuals : MonoBehaviour {
    
    public Color AnimatedColor;

    private Animator animator;
    private Renderer visualsRenderer;

    private int colorPropertyID;
    private MaterialPropertyBlock dynamicProps;
    private bool sheduledForDestruction;

    private void Awake() {
        animator = GetComponent<Animator>();
        dynamicProps = new MaterialPropertyBlock();
        visualsRenderer = GetComponentInChildren<Renderer>();
        colorPropertyID = Shader.PropertyToID("_Color");
    }

    private void Start() {
        animator.SetFloat("CycleOffset", Random.Range(0, 1f));
    }

    void LateUpdate() {
        dynamicProps.SetColor(colorPropertyID, AnimatedColor);
        visualsRenderer.SetPropertyBlock(dynamicProps);

        if (sheduledForDestruction && !IsAnimatorPlaying()) {
            Destroy(gameObject);
        }
    }

    private bool IsAnimatorPlaying() {
        var curentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return animator.IsInTransition(0) || curentStateInfo.normalizedTime < curentStateInfo.length;
    }

    internal void UpdatePositionAndRotation(Vector3 position, Quaternion rotation) {
        transform.SetPositionAndRotation(position, overrideRotation ? overridedQuaterion: rotation);
    }

    private Quaternion overridedQuaterion;
    private bool overrideRotation;

    internal void SetOverrideRotation(Quaternion quaternion) {
        overridedQuaterion = quaternion;
        overrideRotation = true;
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
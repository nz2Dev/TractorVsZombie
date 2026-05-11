using UnityEngine;

[ExecuteInEditMode]
public class InfantryVisuals : MonoBehaviour {
    
    public Color AnimatedColor; //temporaly disabled

    private Animator animator;
    private Renderer visualsRenderer;

    private int hitFlashPropertyID;
    private MaterialPropertyBlock dynamicProps;
    private bool sheduledForDestruction;

    private float hitFlash;
    private float hitFlashBottom = 0;

    private void Awake() {
        animator = GetComponent<Animator>();
        dynamicProps = new MaterialPropertyBlock();
        visualsRenderer = GetComponentInChildren<Renderer>();
        hitFlashPropertyID = Shader.PropertyToID("_HitFlash");
    }

    private void Start() {
        animator.SetFloat("CycleOffset", Random.Range(0, 1f));
    }

    private void Update() {
        hitFlash = Mathf.MoveTowards(hitFlash, hitFlashBottom, Time.deltaTime);
    }

    void LateUpdate() {
        if (dynamicProps != null) {
            dynamicProps.SetFloat(hitFlashPropertyID, hitFlash);
            visualsRenderer.SetPropertyBlock(dynamicProps);
        }

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
        // animator.SetTrigger("Take Hit");
        hitFlash = 1;
    }

    internal void PlayDirectAttackAnimation() {
        animator.SetTrigger("Attack");
    }

    internal void PlayPushedAwayDeathAnimation() {
        animator.SetTrigger("Throw Death");
    }

    internal void PlayDisolveAnimation() {
        animator.SetTrigger("Disolve Death");
        hitFlashBottom = -1;
    }

    internal void DestroySelfOnIdle() {
        sheduledForDestruction = true;
    }
}
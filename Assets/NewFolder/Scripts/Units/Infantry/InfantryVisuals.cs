using UnityEngine;

[ExecuteInEditMode]
public class InfantryVisuals : MonoBehaviour {
    
    public Color AnimatedColor; //temporaly disabled

    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float powerBottom = .3f;

    private Animator animator;
    private Renderer visualsRenderer;

    private float hitFlash;
    private int hitFlashPropertyID;
    private float power = 1;
    private float powerSubtractor = 0;
    private int powerPropertyID;
    private MaterialPropertyBlock dynamicProps;

    private bool sheduledForDestruction;
    private Quaternion currentRotation;
    private Quaternion targetRotation;
    private Quaternion overridedQuaterion;
    private bool overrideRotation;

    private void Awake() {
        animator = GetComponent<Animator>();
        dynamicProps = new MaterialPropertyBlock();
        visualsRenderer = GetComponentInChildren<Renderer>();
        hitFlashPropertyID = Shader.PropertyToID("_HitFlash");
        powerPropertyID = Shader.PropertyToID("_Power");

        currentRotation = transform.rotation;
        targetRotation = currentRotation;
    }

    private void Start() {
        animator.SetFloat("CycleOffset", Random.Range(0, 1f));
    }

    private void Update() {
        hitFlash = Mathf.MoveTowards(hitFlash, 0, Time.deltaTime);
        power = Mathf.MoveTowards(power, powerBottom, Time.deltaTime * powerSubtractor);
        currentRotation = Quaternion.RotateTowards(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void LateUpdate() {
        if (dynamicProps != null) {
            dynamicProps.SetFloat(hitFlashPropertyID, hitFlash);
            dynamicProps.SetFloat(powerPropertyID, Mathf.Clamp01(power));
            visualsRenderer.SetPropertyBlock(dynamicProps);
        }

        if (sheduledForDestruction && !IsAnimatorPlaying() && hitFlash < float.Epsilon && power < powerBottom * 1.1f) {
            Destroy(gameObject);
        }
    }

    private bool IsAnimatorPlaying() {
        var curentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return animator.IsInTransition(0) || curentStateInfo.normalizedTime < curentStateInfo.length;
    }

    internal void UpdatePositionAndRotation(Vector3 position, Quaternion rotation) {
        targetRotation = overrideRotation ? overridedQuaterion : rotation;
        transform.SetPositionAndRotation(position, currentRotation);
    }

    internal void SetOverrideRotation(Quaternion quaternion) {
        overridedQuaterion = quaternion;
        overrideRotation = true;
        currentRotation = quaternion;
        targetRotation = quaternion;
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
        power = 1.5f;
        powerSubtractor = 1;
    }

    internal void PlayDisolveAnimation() {
        animator.SetTrigger("Disolve Death");
        power = 1;
        powerSubtractor = 1;
    }

    internal void DestroySelfOnIdle() {
        sheduledForDestruction = true;
    }

    internal void SetSpeed(float speedNormalized) {
        animator.SetFloat("Speed", speedNormalized);
    }
}
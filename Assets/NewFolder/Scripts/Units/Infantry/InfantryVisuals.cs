using UnityEngine;

// [ExecuteInEditMode]
public class InfantryVisuals : MonoBehaviour {
    
    public Color AnimatedColor; //temporaly disabled

    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float powerBottom = .3f;
    [SerializeField] private Color takeHitColor = Color.red;
    [SerializeField] private Color chargeColor = Color.yellow;
    [SerializeField] private float chargeSinScale = 2f;
    [SerializeField] private float chargeDecaySpeed = 2f;

    private Animator animator;
    private Renderer visualsRenderer;

    private float hitFlash;
    private bool takeHitPlaying;
    private float chargeFlash;
    private float chargeTime;
    private float chargeDecay;
    private bool chargePlaying;
    private int hitFlashPropertyID;
    private float power = 1;
    private float powerSubtractor = 0;
    private int powerPropertyID;
    private MaterialPropertyBlock dynamicProps;
    private int emissionPropertyID;
    private Color emissionColor;

    private bool sheduledForDestruction;
    private Quaternion currentRotation;
    private Quaternion targetRotation;
    private Quaternion overridedQuaterion;
    private bool overrideRotation;

    private void Awake() {
        animator = GetComponent<Animator>();
        dynamicProps = new MaterialPropertyBlock();
        visualsRenderer = GetComponentInChildren<Renderer>();
        emissionPropertyID = Shader.PropertyToID("_HitEmissionColor");
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
        if (hitFlash < float.Epsilon) {
            takeHitPlaying = false;
        }

        chargeTime += Time.deltaTime;
        chargeFlash = (Mathf.Cos(chargeTime * chargeSinScale) + 1) / 2f;
        chargeDecay = Mathf.MoveTowards(chargeDecay, 0, Time.deltaTime * chargeDecaySpeed);
        if (chargeDecay < float.Epsilon) {
            chargePlaying = false;
        }

        power = Mathf.MoveTowards(power, powerBottom, Time.deltaTime * powerSubtractor);
        currentRotation = Quaternion.RotateTowards(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    void LateUpdate() {
        if (dynamicProps != null) {
            var emission = 0f;
            if (chargePlaying) {
                emission = chargeFlash;
            }
            if (takeHitPlaying) {
                emission = hitFlash;
            }
            dynamicProps.SetFloat(hitFlashPropertyID, emission);
            dynamicProps.SetFloat(powerPropertyID, Mathf.Clamp01(power));
            dynamicProps.SetColor(emissionPropertyID, emissionColor);
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
        takeHitPlaying = true;
        emissionColor = takeHitColor;
    }

    internal void PlayCharge() {
        if (!chargePlaying) {
            chargeTime = 0;
        }
        chargeDecay = 1;
        chargePlaying = true;
        emissionColor = chargeColor;
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
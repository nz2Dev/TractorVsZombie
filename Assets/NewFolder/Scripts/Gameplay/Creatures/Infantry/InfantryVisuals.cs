using UnityEngine;

// [ExecuteInEditMode]
public class InfantryVisuals : MonoBehaviour {
    
    public Color AnimatedColor; //temporaly disabled

    [SerializeField] private float rotationSpeed = 720f;
    [Range(0f, 1f)][SerializeField] private float powerMin;
    [Range(0f, 2f)][SerializeField] private float powerMax = 1f;
    [Range(0f, 1f)][SerializeField] private float healthPowerPart = .5f;
    [SerializeField] private Color takeHitColor = Color.red;
    [SerializeField] private Color chargeColor = Color.yellow;
    [SerializeField] private float chargeDecaySpeed = 2f;

    private Animator animator;
    private Renderer visualsRenderer;

    private readonly FloatFadeEffect hitFlash = new FloatFadeEffect();

    private readonly ChargeEffect chargeEffect = new ChargeEffect();
    private Color emissionColor;
    
    private float healthLoad;
    private readonly DissolveEffect dissolveEffect = new DissolveEffect();
    private float power = 1f;

    private MaterialPropertyBlock dynamicProps;
    private int powerPropertyID;
    private int hitFlashPropertyID;
    private int emissionPropertyID;
    private int verticalFillAmountPropertyID;

    private bool sheduledForDestruction;
    private Quaternion currentRotation;
    private Quaternion targetRotation;

    private void Awake() {
        animator = GetComponent<Animator>();
        dynamicProps = new MaterialPropertyBlock();
        visualsRenderer = GetComponentInChildren<Renderer>();
        emissionPropertyID = Shader.PropertyToID("_HitEmissionColor");
        hitFlashPropertyID = Shader.PropertyToID("_HitFlash");
        powerPropertyID = Shader.PropertyToID("_Power");
        verticalFillAmountPropertyID = Shader.PropertyToID("_VerticalFillAmount");

        currentRotation = transform.rotation;
        targetRotation = currentRotation;
    }

    private void Start() {
        animator.SetFloat("CycleOffset", Random.Range(0, 1f));
    }

    private void Update() {
        hitFlash.Tick(Time.deltaTime);

        chargeEffect.Tick(Time.deltaTime);

        dissolveEffect.Tick(Time.deltaTime);
        var healthReserve = 1f - healthLoad;
        var normalizedPower = healthReserve * healthPowerPart + dissolveEffect.Value * (1f - healthPowerPart);
        power = Mathf.Lerp(powerMin, powerMax, normalizedPower);
        
        currentRotation = Quaternion.RotateTowards(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = currentRotation;
    }

    void LateUpdate() {
        if (dynamicProps != null) {
            var emission = 0f;
            var fill = 1f;
            if (chargeEffect.IsPlaying) {
                emission = 1f;
                fill = chargeEffect.Fill;
            }
            if (hitFlash.IsPlaying) {
                emission = hitFlash.Value;
                fill = 1;
            }
            dynamicProps.SetFloat(hitFlashPropertyID, emission);
            dynamicProps.SetFloat(powerPropertyID, Mathf.Clamp01(power));
            dynamicProps.SetFloat(verticalFillAmountPropertyID, fill);
            dynamicProps.SetColor(emissionPropertyID, emissionColor);
            visualsRenderer.SetPropertyBlock(dynamicProps);
        }

        if (sheduledForDestruction && !IsAnimatorPlaying() && !hitFlash.IsPlaying && dissolveEffect.IsComplete) {
            Destroy(gameObject);
        }
    }

    private bool IsAnimatorPlaying() {
        var curentStateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return animator.IsInTransition(0) || curentStateInfo.normalizedTime < 0.95f;
    }

    internal void SetPosition(Vector3 position) {
        transform.position = position;
    }

    internal void SetRotation(Quaternion rotation) {
        currentRotation = rotation;
        targetRotation = rotation;
        transform.rotation = currentRotation;
    }

    internal void SetTargetRotation(Quaternion rotation) {
        targetRotation = rotation;
    }

    internal void PlayTakeHit() {
        // animator.SetTrigger("Take Hit");
        hitFlash.Play();
        emissionColor = takeHitColor;
    }

    internal void PlayCharge(float duration) {
        chargeEffect.Play(duration);
        emissionColor = chargeColor;
    }

    internal void PlayDischarge() {
        chargeEffect.Discharge(chargeDecaySpeed);
    }

    public void OnAttackFinished() {
        chargeEffect.Finish();
    }

    internal void PlayChargeAttack() {
        chargeEffect.PlayFull();
        emissionColor = chargeColor;
        animator.SetTrigger("Attack");
    }

    internal void PlayPushedAwayDeathAnimation() {
        animator.SetTrigger("Throw Death");
        dissolveEffect.Play();
    }

    internal void PlayDisolveAnimation() {
        animator.SetTrigger("Disolve Death");
        dissolveEffect.Play();
    }

    public void SetHealthLoad(float healthLoadNormalized) {
        healthLoad = Mathf.Clamp01(healthLoadNormalized);
    }

    internal void DestroySelfOnIdle() {
        sheduledForDestruction = true;
    }

    internal void SetMovementSpeed(float speedNormalized) {
        animator.SetFloat("Speed", speedNormalized);
    }

}

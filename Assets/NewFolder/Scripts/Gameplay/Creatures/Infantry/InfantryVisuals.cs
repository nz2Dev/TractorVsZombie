using UnityEngine;

[RequireComponent(typeof(EffectsMaterialProperties))]
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
    private EffectsMaterialProperties effectsMaterialProperties;
    private readonly FloatFadeEffect hitFlash = new FloatFadeEffect();
    private readonly ChargeEffect chargeEffect = new ChargeEffect();
    private readonly DissolveEffect dissolveEffect = new DissolveEffect();
    
    private float healthNormalized;
    private bool sheduledForDestruction;
    private Quaternion currentRotation;
    private Quaternion targetRotation;

    private void Awake() {
        animator = GetComponent<Animator>();
        effectsMaterialProperties = GetComponent<EffectsMaterialProperties>();

        currentRotation = transform.rotation;
        targetRotation = currentRotation;
    }

    private void Start() {
        animator.SetFloat("CycleOffset", Random.Range(0, 1f));
    }

    private void Update() {
        effectsMaterialProperties.EmissionIntensity = 0;
        chargeEffect.Tick(Time.deltaTime);
        if (chargeEffect.IsPlaying) {
            effectsMaterialProperties.EmissionIntensity = 1;
            effectsMaterialProperties.EmissionColor = chargeColor;
            effectsMaterialProperties.EmissionFillAmount = chargeEffect.Fill;
        }

        hitFlash.Tick(Time.deltaTime);
        if (hitFlash.IsPlaying) {
            effectsMaterialProperties.EmissionIntensity = hitFlash.Value;
            effectsMaterialProperties.EmissionColor = takeHitColor;
            effectsMaterialProperties.EmissionFillAmount = 1;
        }

        dissolveEffect.Tick(Time.deltaTime);
        var disolvePowerPart = (1f - healthPowerPart);
        var normalizedPower = healthNormalized * healthPowerPart + dissolveEffect.Value * disolvePowerPart;
        var power = Mathf.Lerp(powerMin, powerMax, normalizedPower);
        effectsMaterialProperties.Power = Mathf.Clamp01(power);
        
        currentRotation = Quaternion.RotateTowards(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = currentRotation;
    }

    void LateUpdate() {
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
    }

    internal void PlayCharge(float duration) {
        chargeEffect.Play(duration);
    }

    internal void PlayDischarge() {
        chargeEffect.Discharge(chargeDecaySpeed);
    }

    public void OnAttackFinished() {
        chargeEffect.Finish();
    }

    internal void PlayChargeAttack() {
        chargeEffect.PlayFull();
        animator.SetTrigger("Attack");
    }

    internal void PlayPushedAwayDeathAnimation() {
        animator.SetTrigger("Throw Death");
        dissolveEffect.Play();
    }

    internal void PlayDisolveAnimation() {
        animator.SetTrigger("Disolve Death"); // state and clip is empty
        dissolveEffect.Play();
    }

    public void SetHealthNormalized(float healthNormalized) {
        this.healthNormalized = healthNormalized;
    }

    internal void DestroySelfOnIdle() {
        sheduledForDestruction = true;
    }

    internal void SetMovementSpeed(float speedNormalized) {
        animator.SetFloat("Speed", speedNormalized);
    }

}

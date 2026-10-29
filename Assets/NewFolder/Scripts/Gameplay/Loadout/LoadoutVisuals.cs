using UnityEngine;

[RequireComponent(typeof(EmmisionEffectMaterialProperties))]
public sealed class LoadoutVisuals : MonoBehaviour {

    [SerializeField] private float newFlashThreshold = 0.5f;

    private readonly FloatFadeEffect hitFlash = new FloatFadeEffect();
    private EmmisionEffectMaterialProperties emmisionMaterialProperties;

    private void Awake() {
        emmisionMaterialProperties = GetComponent<EmmisionEffectMaterialProperties>();
    }

    public void TriggerHitFlash() {
        hitFlash.TryPlayBelow(newFlashThreshold);
    }

    private void Update() {
        hitFlash.Tick(Time.deltaTime);
        emmisionMaterialProperties.Flash = hitFlash.Value;
    }
}

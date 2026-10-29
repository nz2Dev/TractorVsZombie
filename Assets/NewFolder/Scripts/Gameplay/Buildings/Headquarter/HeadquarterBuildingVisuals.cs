using UnityEngine;

[RequireComponent(typeof(EmmisionEffectMaterialProperties))]
public class HeadquarterBuildingVisuals : MonoBehaviour {
    
    [SerializeField] private float newFlashThreashold = 0.5f;

    private EmmisionEffectMaterialProperties emmisionEffectMaterialProperties;

    private readonly FloatFadeEffect hitFlash = new FloatFadeEffect();

    private void Awake() {
        emmisionEffectMaterialProperties = GetComponent<EmmisionEffectMaterialProperties>();
    }

    public void TriggerHitFlash() {
        hitFlash.TryPlayBelow(newFlashThreashold);
    }

    private void Update() {
        hitFlash.Tick(Time.deltaTime);
        emmisionEffectMaterialProperties.Flash = hitFlash.Value;
    }

}

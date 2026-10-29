using UnityEngine;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
public sealed class EffectsMaterialProperties : MonoBehaviour {

    [SerializeField] private Renderer[] affectedRenderers = new Renderer[0];
    [Range(0f, 1f)][SerializeField] private float emissionIntensity;
    [SerializeField] private Color emissionColor = Color.red;
    [SerializeField] private float power = 1f;
    [Range(0f, 1f)][SerializeField] private float emissionFillAmount = 1f;

    public float EmissionIntensity {
        get => emissionIntensity;
        set => emissionIntensity = value;
    }

    public Color EmissionColor {
        get => emissionColor;
        set => emissionColor = value;
    }

    public float Power {
        get => power;
        set => power = value;
    }

    public float EmissionFillAmount {
        get => emissionFillAmount;
        set => emissionFillAmount = value;
    }

    private static readonly int HitFlashPropertyId = Shader.PropertyToID("_HitFlash");
    private static readonly int HitEmissionColorPropertyId = Shader.PropertyToID("_HitEmissionColor");
    private static readonly int PowerPropertyId = Shader.PropertyToID("_Power");
    private static readonly int VerticalFillAmountPropertyId = Shader.PropertyToID("_VerticalFillAmount");

    private MaterialPropertyBlock propertyBlock;

    private void Awake() {
        propertyBlock = new MaterialPropertyBlock();
    }

    private void LateUpdate() {
        propertyBlock.SetFloat(HitFlashPropertyId, emissionIntensity);
        propertyBlock.SetColor(HitEmissionColorPropertyId, emissionColor);
        propertyBlock.SetFloat(PowerPropertyId, power);
        propertyBlock.SetFloat(VerticalFillAmountPropertyId, emissionFillAmount);

        foreach (var affectedRenderer in affectedRenderers) {
            if (affectedRenderer != null) {
                affectedRenderer.SetPropertyBlock(propertyBlock);
            }
        }
    }
}

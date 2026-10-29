using UnityEngine;

[DisallowMultipleComponent]
public sealed class EmmisionEffectMaterialProperties : MonoBehaviour {

    [SerializeField] private Renderer[] affectedRenderers = new Renderer[0];
    [Range(0f, 1f)][SerializeField] private float flash;

    private static readonly int FlashPropertyId = Shader.PropertyToID("_Flash");

    private MaterialPropertyBlock propertyBlock;

    public float Flash {
        get => flash;
        set => flash = value;
    }

    private void Awake() {
        propertyBlock = new MaterialPropertyBlock();
    }

    private void LateUpdate() {
        propertyBlock.SetFloat(FlashPropertyId, flash);

        foreach (var affectedRenderer in affectedRenderers) {
            if (affectedRenderer != null) {
                affectedRenderer.SetPropertyBlock(propertyBlock);
            }
        }
    }
}

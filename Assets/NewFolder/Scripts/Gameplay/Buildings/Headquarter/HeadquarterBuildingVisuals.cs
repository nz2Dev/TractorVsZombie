using UnityEngine;
using UnityEngine.Rendering;

public class HeadquarterBuildingVisuals : MonoBehaviour {
    
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private float newFlashThreashold = 0.5f;

    private MaterialPropertyBlock propertyBlock;
    private int flashPropertyId;

    private readonly FloatFadeEffect hitFlash = new FloatFadeEffect();

    private void Awake() {
        propertyBlock = new MaterialPropertyBlock();
        flashPropertyId = Shader.PropertyToID("_Flash");
    }

    public void TriggerHitFlash() {
        hitFlash.TryPlayBelow(newFlashThreashold);
    }

    private void Update() {
        hitFlash.Tick(Time.deltaTime);
        propertyBlock.SetFloat(flashPropertyId, hitFlash.Value);
        meshRenderer.SetPropertyBlock(propertyBlock);
    }

}

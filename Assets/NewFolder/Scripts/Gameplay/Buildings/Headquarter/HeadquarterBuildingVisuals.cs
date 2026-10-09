using UnityEngine;
using UnityEngine.Rendering;

public class HeadquarterBuildingVisuals : MonoBehaviour {
    
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private float newFlashThreashold = 0.5f;

    private MaterialPropertyBlock propertyBlock;
    private int flashPropertyId;

    private float flash;

    private void Awake() {
        propertyBlock = new MaterialPropertyBlock();
        flashPropertyId = Shader.PropertyToID("_Flash");
    }

    public void TriggerHitFlash() {
        if (flash < newFlashThreashold)
            flash = 1;
    }

    private void Update() {
        flash = Mathf.MoveTowards(flash, 0, Time.deltaTime);
        propertyBlock.SetFloat(flashPropertyId, flash);
        meshRenderer.SetPropertyBlock(propertyBlock);
    }

}
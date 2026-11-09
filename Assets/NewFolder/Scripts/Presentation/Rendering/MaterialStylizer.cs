using UnityEngine;

public class MaterialStylizer : MonoBehaviour {
    
    [NonNull, SerializeField] private Renderer[] targets = new Renderer[0];
    [NonNull, SerializeField] private MaterialStyleData data;
    [SerializeField] private MaterialStyle style;

    public MaterialStyle Style {
        set { 
            style = value;
            UpdateSharedMaterial();
        }
    }

    private void OnValidate() {
        if (data != null) {
            UpdateSharedMaterial();
        }
    }

    private void UpdateSharedMaterial() {
        foreach (var renderer in targets) {
            renderer.sharedMaterial = data.GetMaterial(style);
        }
    }

}
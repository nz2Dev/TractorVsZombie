using UnityEngine;

[CreateAssetMenu(fileName = "MaterialStyle", menuName = "MaterialStyle", order = 0)]
public class MaterialStyleData : ScriptableObject {
    
    [SerializeField] private Material foeAccentMaterial;
    [SerializeField] private Material alieAccentMaterial;
    [SerializeField] private Material neutralAccentMaterial;

    public Material GetMaterial(MaterialStyle style) => style switch {
        MaterialStyle.FactionFoe => foeAccentMaterial,
        MaterialStyle.FactionAlie => alieAccentMaterial,
        _ => neutralAccentMaterial,
    };
}
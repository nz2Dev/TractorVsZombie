using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class RadialSlider : MonoBehaviour {

    static readonly int FillId = Shader.PropertyToID("_Fill");
    
    [SerializeField] private Image image;
    [SerializeField] private float value;

    private Material instance;

    public float Value {
        set => this.value = value;
        get => value;
    }

    private void Awake() {
        if (image != null) {
            instance = new Material(image.material);
            image.material = instance;
        }
    }

    private void LateUpdate() {
        if (instance != null) {
            instance.SetFloat(FillId, value);
        }
    }

    void OnDestroy() {
        if (instance != null) {
            Destroy(instance);
        }
    }
    
}
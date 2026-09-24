using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Drives the "UI/RadialDiamondFill" shader on an Image.
/// Creates a per-object material instance so each instance can have its own fill.
/// </summary>
[RequireComponent(typeof(Image))]
public class RadialDiamondFill : MonoBehaviour
{
    static readonly int FillId = Shader.PropertyToID("_Fill");

    [SerializeField, Range(0f, 1f)] float fill = 1f;

    Image image;
    Material instance;

    public float Fill
    {
        get => fill;
        set
        {
            fill = Mathf.Clamp01(value);
            Apply();
        }
    }

    void Awake()
    {
        image = GetComponent<Image>();
        instance = new Material(image.material);
        image.material = instance;
        Apply();
    }

    void OnDestroy()
    {
        if (instance != null) Destroy(instance);
    }

#if UNITY_EDITOR
    // Lets you scrub the slider in the Inspector during Play Mode.
    void OnValidate()
    {
        if (instance != null) Apply();
    }
#endif

    void Apply()
    {
        if (instance != null) instance.SetFloat(FillId, fill);
    }
}

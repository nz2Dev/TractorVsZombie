using System;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class HealthBarVisuals : MonoBehaviour {
    
    static readonly int FillId = Shader.PropertyToID("_Fill");

    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI text;

    private Material instance;

    private void Awake() {
        if (image != null) {
            instance = new Material(image.material);
            image.material = instance;
        }
    }

    public void SetBar(int health, int maxHealth) {
        text.text = $"[{health} / {maxHealth}]";
        if (instance != null) {
            instance.SetFloat(FillId, (float) health / Mathf.Max(maxHealth, 1));
        }
    }

     void OnDestroy() {
        if (instance != null)
            Destroy(instance);
    }

}
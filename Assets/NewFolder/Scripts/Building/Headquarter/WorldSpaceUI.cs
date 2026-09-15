using NUnit.Framework;

using UnityEngine;

public class WorldSpaceUI : MonoBehaviour {
    
    [SerializeField] private Canvas canvas;

    public Camera TargetCamera { get; set; }

    private void Start() {
        Assert.NotNull(TargetCamera);
    }

    private void Update() {
        canvas.transform.rotation = TargetCamera.transform.rotation;
    }

    [ContextMenu("Adjust to available camera")]
    private void AdjustToAvailableCamera() {
        canvas.transform.rotation = Camera.main.transform.rotation;
    }

}
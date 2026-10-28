using System;

using UnityEngine;

public class TruckVisuals : MonoBehaviour {
    
    [Serializable]
    public struct WheelAxis {
        public GameObject leftWheel;
        public GameObject rightWheel;
    }

    [SerializeField] private WheelAxis frontAxis;
    [SerializeField] private WheelAxis rearAxis;

    [SerializeField] private Material alieUnitMaterial;
    [SerializeField] private Material foeUnitMaterial;
    [SerializeField] private MeshRenderer[] meshRendererers;
    [SerializeField] private float newFlashThreashold = 0.5f;
    private MaterialPropertyBlock propertyBlock;
    private int flashPropertyId;
    private float flash;

    private void Awake() {
        propertyBlock = new MaterialPropertyBlock();
        flashPropertyId = Shader.PropertyToID("_Flash");
    }

    public void SetFactionProperties(bool alie) {
        foreach (var renderer in meshRendererers) {
            renderer.sharedMaterial = alie ? alieUnitMaterial : foeUnitMaterial;
        }
    }

    public void SetPositionAndRotation(Vector3 pos, Quaternion rot) {
        transform.SetPositionAndRotation(pos, rot);
    }

    public void SetFrontAxis(WheelAxisPose axisPose) {
        frontAxis.leftWheel.transform.SetPositionAndRotation(axisPose.positionL, axisPose.rotationL);
        frontAxis.rightWheel.transform.SetPositionAndRotation(axisPose.positionR, axisPose.rotationR);
    }
    
    public void SetRearAxis(WheelAxisPose axisPose) {
        rearAxis.leftWheel.transform.SetPositionAndRotation(axisPose.positionL, axisPose.rotationL);
        rearAxis.rightWheel.transform.SetPositionAndRotation(axisPose.positionR, axisPose.rotationR);
    }

    public void TriggerHitFlash() {
        if (flash < newFlashThreashold)
            flash = 1;
    }

    private void Update() {
        flash = Mathf.MoveTowards(flash, 0, Time.deltaTime);
        propertyBlock.SetFloat(flashPropertyId, flash);

        foreach (var renderer in meshRendererers) {
            renderer.SetPropertyBlock(propertyBlock);
        }
    }

}
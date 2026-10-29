using System;

using UnityEngine;

[RequireComponent(typeof(EmmisionEffectMaterialProperties))]
public class ArmorVisuals : MonoBehaviour {
    
    [Serializable]
    public struct WheelAxis {
        public GameObject leftWheel;
        public GameObject rightWheel;
    }

    [SerializeField] private WheelAxis frontAxis;
    [SerializeField] private WheelAxis rearAxis;
    [SerializeField] private float newFlashThreashold = 0.5f;
    
    private readonly FloatFadeEffect hitFlash = new FloatFadeEffect();
    private EmmisionEffectMaterialProperties emmisionEffectMaterialProperties;

    private void Awake() {
        emmisionEffectMaterialProperties = GetComponent<EmmisionEffectMaterialProperties>();
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
        hitFlash.TryPlayBelow(newFlashThreashold);
    }

    private void Update() {
        hitFlash.Tick(Time.deltaTime);
        emmisionEffectMaterialProperties.Flash = hitFlash.Value;
    }

}

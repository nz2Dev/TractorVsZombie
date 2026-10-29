using UnityEngine;

[RequireComponent(typeof(EmmisionEffectMaterialProperties), typeof(VehicleWheelRig))]
public class PlatformVisuals : MonoBehaviour {
    
    [SerializeField] private float newFlashThreashold = 0.5f;

    private readonly FloatFadeEffect hitFlash = new FloatFadeEffect();
    private EmmisionEffectMaterialProperties emmisionEffectMaterialProperties;
    private VehicleWheelRig wheelRig;

    private void Awake() {
        emmisionEffectMaterialProperties = GetComponent<EmmisionEffectMaterialProperties>();
        wheelRig = GetComponent<VehicleWheelRig>();
    }

    public void SetPositionAndRotation(Vector3 pos, Quaternion rot) {
        transform.SetPositionAndRotation(pos, rot);
    }

    public void SetFrontAxis(WheelAxisPose axisPose) {
        wheelRig.SetFrontAxis(axisPose);
    }
    
    public void SetRearAxis(WheelAxisPose axisPose) {
        wheelRig.SetRearAxis(axisPose);
    }

    public void TriggerHitFlash() {
        hitFlash.TryPlayBelow(newFlashThreashold);
    }

    private void Update() {
        hitFlash.Tick(Time.deltaTime);
        emmisionEffectMaterialProperties.Flash = hitFlash.Value;
    }
}

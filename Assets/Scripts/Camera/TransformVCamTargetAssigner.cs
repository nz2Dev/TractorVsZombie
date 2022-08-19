using Cinemachine;
using UnityEngine;

[ExecuteInEditMode]
public class TransformVCamTargetAssigner : MonoBehaviour {
    
    [SerializeField] private Transform transformToAssign;

    [ContextMenu("Bind VCam targets")]
    private void Awake() {
        var vcam = GetComponent<CinemachineVirtualCamera>();
        vcam.Follow = transformToAssign;
        vcam.LookAt = transformToAssign;
    }
}
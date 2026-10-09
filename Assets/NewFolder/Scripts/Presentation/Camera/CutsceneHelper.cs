using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class CutsceneHelper : MonoBehaviour {
    
    [SerializeField] private Transform targetTransform;

    [ContextMenu("Adjust Top Down Position")]
    private void AdjustTopDownCameraPosition() {
        GameObject.FindFirstObjectByType<CameraManager>().UpdateTopDownFollowPosition(targetTransform.position);
    }

    [ContextMenu("Rebind Cinemachine Brain")]
    private void RebindCinemachineBrain() {
        GameObject.FindFirstObjectByType<CameraManager>().BindToDirector(GetComponent<PlayableDirector>());
    }

}